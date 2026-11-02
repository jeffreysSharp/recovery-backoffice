import { useEffect, useState, type FormEvent } from "react";
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Globe, Plus } from "lucide-react";
import { toast } from "sonner";
import { createCity, deleteCity, searchCities, updateCity, type CityDto, type CityInput, loadCityCountries, loadCityStates } from "@/api/cities";
import { useAuth } from "@/auth/use-auth";
import { CitiesPermissions } from "@/lib/permissions";
import { ApiRequestError, describeError } from "@/lib/api-client";
import { EntityPageHeader, Field, Pagination } from "@/components/list";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Dialog, DialogBody, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

const blank: CityInput = { stateId: "", name: "", ibgeCode: "", isActive: true };
const fields = [["name", "Cidade", 150], ["ibgeCode", "Código IBGE", 7]] as const;

export function CitiesPage() {
  const { user } = useAuth();
  const can = (permission: string) => user?.permissions.includes(permission) ?? false;
  const client = useQueryClient();
  const [search, setSearch] = useState("");
  const [term, setTerm] = useState("");
  const [page, setPage] = useState(1);
  const [active, setActive] = useState("");
  const [countryFilter, setCountryFilter] = useState("");
  const [stateFilter, setStateFilter] = useState("");
  const [sortBy, setSortBy] = useState("name");
  const [sortDir, setSortDir] = useState("asc");
  const [editor, setEditor] = useState<CityDto | "new" | null>(null);
  const [deleting, setDeleting] = useState<CityDto | null>(null);
  const [toggling, setToggling] = useState<CityDto | null>(null);

  useEffect(() => {
    const timer = setTimeout(() => { setTerm(search.trim()); setPage(1); }, 250);
    return () => clearTimeout(timer);
  }, [search]);

  const query = useQuery({
    queryKey: ["cities", { term, page, active, countryFilter, stateFilter, sortBy, sortDir }],
    queryFn: () => searchCities({ search: term || undefined, countryId: countryFilter || undefined, stateId: stateFilter || undefined, pageNumber: page, isActive: active === "" ? undefined : active === "true", sortBy, sortDir }),
    placeholderData: keepPreviousData,
  });
  const refresh = () => client.invalidateQueries({ queryKey: ["cities"] });
  const deletion = useMutation({
    mutationFn: (city: CityDto) => deleteCity(city.id),
    onSuccess: () => { toast.success("Cidade excluída."); setDeleting(null); setPage(1); void refresh(); },
    onError: (error) => toast.error("Não foi possível excluir a cidade.", { description: describeError(error) }),
  });
  const toggle = useMutation({
    mutationFn: (city: CityDto) => updateCity(city.id, { stateId: city.stateId, name: city.name, ibgeCode: city.ibgeCode, isActive: !city.isActive }),
    onSuccess: () => { toast.success("Situação atualizada."); setToggling(null); void refresh(); },
    onError: (error) => toast.error("Não foi possível alterar a situação.", { description: describeError(error) }),
  });
  const countryQuery = useQuery({ queryKey: ["cities-country-options"], queryFn: loadCityCountries });
  const countryOptions = countryQuery.data ?? [];
  const stateQuery = useQuery({ queryKey: ["cities-state-options", countryFilter], queryFn: () => loadCityStates(countryFilter || undefined) });
  const data = query.data;
  const items = data?.items ?? [];
  return (
    <div className="space-y-5">
      <EntityPageHeader icon={Globe} title="Cidades" total={data?.totalCount} unit="cidade" unitPlural="cidades" description="Catálogo global compartilhado por todas as agências.">
        {can(CitiesPermissions.Create) && <Button onClick={() => setEditor("new")}><Plus className="mr-2 h-4 w-4" />Nova cidade</Button>}
      </EntityPageHeader>
      <div className="flex flex-wrap gap-3">
        <Input aria-label="Pesquisar cidades" placeholder="Pesquisar por cidade ou código IBGE…" value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-sm" />
        <Select label="País" value={countryFilter} onChange={(value) => { setCountryFilter(value); setStateFilter(""); setPage(1); }} placeholder="Todos os países" disabled={countryQuery.isLoading || countryQuery.isError} options={countryOptions.map(country => ({ value: country.id, label: country.name }))} />
        <Select label="Estado" value={stateFilter} onChange={value => { setStateFilter(value); setPage(1); }} placeholder="Todos os estados" disabled={stateQuery.isLoading || stateQuery.isError} options={(stateQuery.data ?? []).map(state => ({ value: state.id, label: state.name }))} />
        <Select label="Situação" value={active} onChange={(value) => { setActive(value); setPage(1); }} placeholder="Todas" options={[{value:"true", label:"Ativos"}, {value:"false", label:"Inativos"}]} />
        <Select label="Ordenar por" value={sortBy} onChange={(value) => { setSortBy(value); setPage(1); }} options={[{value:"name",label:"Cidade"},{value:"stateName",label:"Estado"},{value:"ibgeCode",label:"Código IBGE"},{value:"countryName",label:"País"},{value:"isActive",label:"Situação"}]} />
        <Select label="Ordem" value={sortDir} onChange={(value) => { setSortDir(value); setPage(1); }} options={[{value:"asc",label:"Crescente"},{value:"desc",label:"Decrescente"}]} />
      </div>
      {stateQuery.isError && <p role="alert">Não foi possível carregar os estados. {describeError(stateQuery.error)}</p>}
      {countryQuery.isError && <p role="alert">Não foi possível carregar os países. {describeError(countryQuery.error)}</p>}
      {query.isError ? <div role="alert" className="text-[var(--color-destructive)]">Não foi possível carregar as cidades. {describeError(query.error)} <Button variant="outline" onClick={() => void query.refetch()}>Tentar novamente</Button></div> :
       query.isLoading ? <p role="status">Carregando cidades…</p> : (
        <div className="rounded-xl border border-[var(--color-border)] bg-[var(--color-card)]">
          <Table>
            <TableHeader><TableRow>{["Cidade","Estado","País","Código IBGE","Situação","Ações"].map(label => <TableHead key={label}>{label}</TableHead>)}</TableRow></TableHeader>
            <TableBody>
              {items.length === 0 && <TableRow><TableCell colSpan={6}>Nenhuma cidade encontrada.</TableCell></TableRow>}
              {items.map(city => <TableRow key={city.id}>
                <TableCell>{city.name}</TableCell><TableCell>{city.stateName}</TableCell>
                <TableCell>{city.countryName}</TableCell><TableCell>{city.ibgeCode ?? "—"}</TableCell><TableCell>{city.isActive ? "Ativo" : "Inativo"}</TableCell>
                <TableCell><div className="flex gap-2">
                  {can(CitiesPermissions.Update) && <><Button size="sm" variant="outline" aria-label={"Editar " + city.name} onClick={() => setEditor(city)}>Editar</Button>
                    <Button size="sm" variant="outline" onClick={() => setToggling(city)}>{city.isActive ? "Desativar" : "Ativar"}</Button></>}
                  {can(CitiesPermissions.Delete) && <Button size="sm" variant="outline" aria-label={"Excluir " + city.name} onClick={() => setDeleting(city)}>Excluir</Button>}
                </div></TableCell>
              </TableRow>)}
            </TableBody>
          </Table>
        </div>)}
      {data && !query.isError && <Pagination page={page} totalPages={data.totalPages} totalCount={data.totalCount} shown={items.length} fetching={query.isFetching} hasPrev={page > 1} hasNext={page < data.totalPages} onPrev={() => setPage(p => p - 1)} onNext={() => setPage(p => p + 1)} noun="cidades" locale="pt-BR" />}
      {editor !== null && <CityEditor key={editor === "new" ? "new" : editor.id} city={editor === "new" ? undefined : editor} countries={countryOptions} countriesLoading={countryQuery.isLoading} countriesError={countryQuery.isError} onClose={() => setEditor(null)} />}
      <ConfirmDialog open={deleting !== null} onOpenChange={(open) => { if (!open) setDeleting(null); }} title="Excluir cidade" description={"Deseja excluir " + (deleting?.name ?? "") + "? Cidades vinculadas a outros registros não podem ser excluídas."} confirmLabel="Excluir" cancelLabel="Cancelar" pendingLabel="Excluindo…" pending={deletion.isPending} destructive onConfirm={() => { if (deleting) deletion.mutate(deleting); }} />
      <ConfirmDialog open={toggling !== null} onOpenChange={(open) => { if (!open) setToggling(null); }} title={toggling?.isActive ? "Desativar cidade" : "Ativar cidade"} description={"Confirma a alteração da situação de " + (toggling?.name ?? "") + "?"} confirmLabel="Confirmar" cancelLabel="Cancelar" pendingLabel="Salvando…" pending={toggle.isPending} onConfirm={() => { if (toggling) toggle.mutate(toggling); }} />
    </div>
  );
}

function CityEditor({city, countries, countriesLoading, countriesError, onClose}: {city?: CityDto; countries: Awaited<ReturnType<typeof loadCityCountries>>; countriesLoading: boolean; countriesError: boolean; onClose: () => void}) {
  const client = useQueryClient();
  const [values, setValues] = useState<CityInput>(city ? { stateId: city.stateId, name: city.name, ibgeCode: city.ibgeCode, isActive: city.isActive } : blank);
  const [countryId, setCountryId] = useState(city?.countryId ?? "");
  const stateQuery = useQuery({ queryKey: ["cities-editor-states", countryId], queryFn: () => loadCityStates(countryId), enabled: countryId !== "" });
  const isBrazil = countries.find(country => country.id === countryId)?.iso2Code === "BR";
  const [errors, setErrors] = useState<Record<string,string>>({});
  const [failure, setFailure] = useState("");
  const mutation = useMutation({
    mutationFn: (input: CityInput) => city ? updateCity(city.id, input) : createCity(input),
    onSuccess: () => { toast.success(city ? "Cidade atualizada." : "Cidade cadastrada."); void client.invalidateQueries({ queryKey: ["cities"] }); onClose(); },
    onError: (error) => {
      setFailure(describeError(error));
      if (error instanceof ApiRequestError && error.problem?.errors && !Array.isArray(error.problem.errors)) {
        const fieldErrors: Record<string,string> = {};
        for (const [key,messages] of Object.entries(error.problem.errors)) {
          fieldErrors[key.charAt(0).toLowerCase() + key.slice(1)] = messages.join(" ");
        }
        setErrors(fieldErrors);
      }
    },
  });
  const submit = (event: FormEvent) => {
    event.preventDefault();
    const input = { ...values, name: values.name.trim().normalize("NFC"), ibgeCode: values.ibgeCode?.trim() || null };
    const next: Record<string,string> = {};
    const brazil = isBrazil;
    if (!countryId) next.countryId = "Selecione um país.";
    if (input.name.length < 2 || input.name.length > 150) next.name = "Cidade deve ter entre 2 e 150 caracteres.";
    if (!input.stateId) next.stateId = "Selecione um estado.";
    if (brazil && !input.ibgeCode) next.ibgeCode = "Código IBGE é obrigatório para cidades brasileiras.";
    else if (input.ibgeCode && !/^[0-9]{7}$/.test(input.ibgeCode)) next.ibgeCode = "Código IBGE deve conter exatamente 7 dígitos.";
    setErrors(next); setFailure("");
    if (!Object.keys(next).length) mutation.mutate(input);
  };
  return <Dialog open onOpenChange={(open) => { if (!open && !mutation.isPending) onClose(); }}>
    <DialogContent><form onSubmit={submit} noValidate>
      <DialogHeader><DialogTitle>{city ? "Editar cidade" : "Nova cidade"}</DialogTitle><DialogDescription>Informe os dados da cidade no catálogo global.</DialogDescription></DialogHeader>
      <DialogBody className="space-y-4">
        {failure && <p role="alert" className="text-[var(--color-destructive)]">{failure}</p>}
        <Field id="city-country" label="País" required error={errors.countryId}>
          <Select id="city-country" value={countryId} onChange={value => { setCountryId(value); setValues(v => ({ ...v, stateId: "" })); }} placeholder="Selecione um país" disabled={mutation.isPending || countriesLoading || countriesError} options={countries.map(country => ({ value: country.id, label: country.name }))} />
        </Field>
        <Field id="city-state" label="Estado" required error={errors.stateId}>
          <Select id="city-state" value={values.stateId} onChange={stateId => setValues(v => ({ ...v, stateId }))} placeholder="Selecione um estado" disabled={mutation.isPending || !countryId || stateQuery.isFetching || stateQuery.isError} options={(stateQuery.data ?? []).map(state => ({ value: state.id, label: state.name }))} />
        </Field>
        {stateQuery.isError && <p role="alert">Não foi possível carregar os estados.</p>}
        {countriesError && <p role="alert">Não foi possível carregar os países.</p>}
        {fields.map(([key,label,max]) => <Field key={key} id={"city-"+key} label={label} required={key==="name" || isBrazil} error={errors[key]}>
          <Input id={"city-"+key} value={values[key] ?? ""} maxLength={max} disabled={mutation.isPending} onChange={event => setValues(v => ({...v,[key]:event.target.value}))} autoFocus={key==="name"} />
        </Field>)}
        <Field id="city-active" label="Ativo"><Switch id="city-active" checked={values.isActive} onCheckedChange={(isActive) => setValues(v => ({...v,isActive}))} disabled={mutation.isPending} /></Field>
      </DialogBody>
      <DialogFooter><Button type="button" variant="outline" disabled={mutation.isPending} onClick={onClose}>Cancelar</Button><Button type="submit" disabled={mutation.isPending || countriesLoading || countriesError || stateQuery.isFetching || stateQuery.isError}>{mutation.isPending ? "Salvando…" : "Salvar"}</Button></DialogFooter>
    </form></DialogContent>
  </Dialog>;
}

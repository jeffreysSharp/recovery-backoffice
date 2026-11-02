import { useEffect, useState, type FormEvent } from "react";
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Globe, Plus } from "lucide-react";
import { toast } from "sonner";
import { createState, deleteState, searchStates, updateState, type StateDto, type StateInput, loadStateCountries } from "@/api/states";
import { useAuth } from "@/auth/use-auth";
import { StatesPermissions } from "@/lib/permissions";
import { ApiRequestError, describeError } from "@/lib/api-client";
import { EntityPageHeader, Field, Pagination } from "@/components/list";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Dialog, DialogBody, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

const blank: StateInput = { countryId: "", name: "", code: "", ibgeCode: "", isActive: true };
const fields = [["name", "Estado", 100], ["code", "Sigla", 10], ["ibgeCode", "Código IBGE", 2]] as const;

export function StatesPage() {
  const { user } = useAuth();
  const can = (permission: string) => user?.permissions.includes(permission) ?? false;
  const client = useQueryClient();
  const [search, setSearch] = useState("");
  const [term, setTerm] = useState("");
  const [page, setPage] = useState(1);
  const [active, setActive] = useState("");
  const [countryFilter, setCountryFilter] = useState("");
  const [sortBy, setSortBy] = useState("name");
  const [sortDir, setSortDir] = useState("asc");
  const [editor, setEditor] = useState<StateDto | "new" | null>(null);
  const [deleting, setDeleting] = useState<StateDto | null>(null);
  const [toggling, setToggling] = useState<StateDto | null>(null);

  useEffect(() => {
    const timer = setTimeout(() => { setTerm(search.trim()); setPage(1); }, 250);
    return () => clearTimeout(timer);
  }, [search]);

  const query = useQuery({
    queryKey: ["states", { term, page, active, countryFilter, sortBy, sortDir }],
    queryFn: () => searchStates({ search: term || undefined, countryId: countryFilter || undefined, pageNumber: page, isActive: active === "" ? undefined : active === "true", sortBy, sortDir }),
    placeholderData: keepPreviousData,
  });
  const refresh = () => client.invalidateQueries({ queryKey: ["states"] });
  const deletion = useMutation({
    mutationFn: (state: StateDto) => deleteState(state.id),
    onSuccess: () => { toast.success("Estado excluído."); setDeleting(null); setPage(1); void refresh(); },
    onError: (error) => toast.error("Não foi possível excluir o estado.", { description: describeError(error) }),
  });
  const toggle = useMutation({
    mutationFn: (state: StateDto) => updateState(state.id, { ...state, isActive: !state.isActive }),
    onSuccess: () => { toast.success("Situação atualizada."); setToggling(null); void refresh(); },
    onError: (error) => toast.error("Não foi possível alterar a situação.", { description: describeError(error) }),
  });
  const countryQuery = useQuery({ queryKey: ["states-country-options"], queryFn: loadStateCountries });
  const countryOptions = countryQuery.data ?? [];
  const data = query.data;
  const items = data?.items ?? [];
  return (
    <div className="space-y-5">
      <EntityPageHeader icon={Globe} title="Estados" total={data?.totalCount} unit="estado" unitPlural="estados" description="Catálogo global compartilhado por todas as agências.">
        {can(StatesPermissions.Create) && <Button onClick={() => setEditor("new")}><Plus className="mr-2 h-4 w-4" />Novo estado</Button>}
      </EntityPageHeader>
      <div className="flex flex-wrap gap-3">
        <Input aria-label="Pesquisar estados" placeholder="Pesquisar por estado, sigla ou código IBGE…" value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-sm" />
        <Select label="País" value={countryFilter} onChange={(value) => { setCountryFilter(value); setPage(1); }} placeholder="Todos os países" disabled={countryQuery.isLoading || countryQuery.isError} options={countryOptions.map(country => ({ value: country.id, label: country.name }))} />
        <Select label="Situação" value={active} onChange={(value) => { setActive(value); setPage(1); }} placeholder="Todas" options={[{value:"true", label:"Ativos"}, {value:"false", label:"Inativos"}]} />
        <Select label="Ordenar por" value={sortBy} onChange={(value) => { setSortBy(value); setPage(1); }} options={[{value:"name",label:"Estado"},{value:"code",label:"Sigla"},{value:"ibgeCode",label:"Código IBGE"},{value:"countryName",label:"País"},{value:"isActive",label:"Situação"}]} />
        <Select label="Ordem" value={sortDir} onChange={(value) => { setSortDir(value); setPage(1); }} options={[{value:"asc",label:"Crescente"},{value:"desc",label:"Decrescente"}]} />
      </div>
      {countryQuery.isError && <p role="alert">Não foi possível carregar os países. {describeError(countryQuery.error)}</p>}
      {query.isError ? <div role="alert" className="text-[var(--color-destructive)]">Não foi possível carregar os estados. {describeError(query.error)} <Button variant="outline" onClick={() => void query.refetch()}>Tentar novamente</Button></div> :
       query.isLoading ? <p role="status">Carregando estados…</p> : (
        <div className="rounded-xl border border-[var(--color-border)] bg-[var(--color-card)]">
          <Table>
            <TableHeader><TableRow>{["Estado","Sigla","Código IBGE","País","Situação","Ações"].map(label => <TableHead key={label}>{label}</TableHead>)}</TableRow></TableHeader>
            <TableBody>
              {items.length === 0 && <TableRow><TableCell colSpan={6}>Nenhum estado encontrado.</TableCell></TableRow>}
              {items.map(state => <TableRow key={state.id}>
                <TableCell>{state.name}</TableCell><TableCell>{state.code}</TableCell>
                <TableCell>{state.ibgeCode ?? "—"}</TableCell><TableCell>{state.countryName}</TableCell><TableCell>{state.isActive ? "Ativo" : "Inativo"}</TableCell>
                <TableCell><div className="flex gap-2">
                  {can(StatesPermissions.Update) && <><Button size="sm" variant="outline" aria-label={"Editar " + state.name} onClick={() => setEditor(state)}>Editar</Button>
                    <Button size="sm" variant="outline" onClick={() => setToggling(state)}>{state.isActive ? "Desativar" : "Ativar"}</Button></>}
                  {can(StatesPermissions.Delete) && <Button size="sm" variant="outline" aria-label={"Excluir " + state.name} onClick={() => setDeleting(state)}>Excluir</Button>}
                </div></TableCell>
              </TableRow>)}
            </TableBody>
          </Table>
        </div>)}
      {data && !query.isError && <Pagination page={page} totalPages={data.totalPages} totalCount={data.totalCount} shown={items.length} fetching={query.isFetching} hasPrev={page > 1} hasNext={page < data.totalPages} onPrev={() => setPage(p => p - 1)} onNext={() => setPage(p => p + 1)} noun="estados" locale="pt-BR" />}
      {editor !== null && <StateEditor key={editor === "new" ? "new" : editor.id} state={editor === "new" ? undefined : editor} countries={countryOptions} countriesLoading={countryQuery.isLoading} countriesError={countryQuery.isError} onClose={() => setEditor(null)} />}
      <ConfirmDialog open={deleting !== null} onOpenChange={(open) => { if (!open) setDeleting(null); }} title="Excluir estado" description={"Deseja excluir " + (deleting?.name ?? "") + "? Estados vinculados a outros registros não podem ser excluídos."} confirmLabel="Excluir" cancelLabel="Cancelar" pendingLabel="Excluindo…" pending={deletion.isPending} destructive onConfirm={() => { if (deleting) deletion.mutate(deleting); }} />
      <ConfirmDialog open={toggling !== null} onOpenChange={(open) => { if (!open) setToggling(null); }} title={toggling?.isActive ? "Desativar estado" : "Ativar estado"} description={"Confirma a alteração da situação de " + (toggling?.name ?? "") + "?"} confirmLabel="Confirmar" cancelLabel="Cancelar" pendingLabel="Salvando…" pending={toggle.isPending} onConfirm={() => { if (toggling) toggle.mutate(toggling); }} />
    </div>
  );
}

function StateEditor({state, countries, countriesLoading, countriesError, onClose}: {state?: StateDto; countries: Awaited<ReturnType<typeof loadStateCountries>>; countriesLoading: boolean; countriesError: boolean; onClose: () => void}) {
  const client = useQueryClient();
  const [values, setValues] = useState<StateInput>(state ?? blank);
  const [errors, setErrors] = useState<Record<string,string>>({});
  const [failure, setFailure] = useState("");
  const mutation = useMutation({
    mutationFn: (input: StateInput) => state ? updateState(state.id, input) : createState(input),
    onSuccess: () => { toast.success(state ? "Estado atualizado." : "Estado cadastrado."); void client.invalidateQueries({ queryKey: ["states"] }); onClose(); },
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
    const input = { ...values, name: values.name.trim(), code: values.code.trim().toUpperCase(), ibgeCode: values.ibgeCode?.trim() || null };
    const next: Record<string,string> = {};
    const brazil = countries.find(country => country.id === values.countryId)?.iso2Code === "BR";
    if (!input.countryId) next.countryId = "Selecione um país.";
    if (input.name.length < 2 || input.name.length > 100) next.name = "Estado deve ter entre 2 e 100 caracteres.";
    if (input.code.length < 1 || input.code.length > 10) next.code = "Sigla deve ter entre 1 e 10 caracteres.";
    if (input.ibgeCode && (!brazil || !/^[0-9]{2}$/.test(input.ibgeCode))) next.ibgeCode = "Código IBGE é exclusivo do Brasil e deve conter 2 dígitos.";
    setErrors(next); setFailure("");
    if (!Object.keys(next).length) mutation.mutate(input);
  };
  return <Dialog open onOpenChange={(open) => { if (!open && !mutation.isPending) onClose(); }}>
    <DialogContent><form onSubmit={submit} noValidate>
      <DialogHeader><DialogTitle>{state ? "Editar estado" : "Novo estado"}</DialogTitle><DialogDescription>Informe os dados do estado no catálogo global.</DialogDescription></DialogHeader>
      <DialogBody className="space-y-4">
        {failure && <p role="alert" className="text-[var(--color-destructive)]">{failure}</p>}
        <Field id="state-country" label="País" required error={errors.countryId}>
          <Select id="state-country" value={values.countryId} onChange={countryId => setValues(v => ({ ...v, countryId, ibgeCode: countries.find(country => country.id === countryId)?.iso2Code === "BR" ? v.ibgeCode : null }))} placeholder="Selecione um país" disabled={mutation.isPending || countriesLoading || countriesError} options={countries.map(country => ({ value: country.id, label: country.name }))} />
        </Field>
        {countriesError && <p role="alert">Não foi possível carregar os países.</p>}
        {fields.map(([key,label,max]) => <Field key={key} id={"state-"+key} label={label} required={key!=="ibgeCode"} error={errors[key]}>
          <Input id={"state-"+key} value={values[key] ?? ""} maxLength={max} disabled={mutation.isPending || (key==="ibgeCode" && countries.find(country => country.id === values.countryId)?.iso2Code !== "BR")} onChange={event => setValues(v => ({...v,[key]:event.target.value}))} autoFocus={key==="name"} />
        </Field>)}
        <Field id="state-active" label="Ativo"><Switch id="state-active" checked={values.isActive} onCheckedChange={(isActive) => setValues(v => ({...v,isActive}))} disabled={mutation.isPending} /></Field>
      </DialogBody>
      <DialogFooter><Button type="button" variant="outline" disabled={mutation.isPending} onClick={onClose}>Cancelar</Button><Button type="submit" disabled={mutation.isPending || countriesLoading || countriesError}>{mutation.isPending ? "Salvando…" : "Salvar"}</Button></DialogFooter>
    </form></DialogContent>
  </Dialog>;
}

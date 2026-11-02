import { useEffect, useState, type FormEvent } from "react";
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Globe, Plus } from "lucide-react";
import { toast } from "sonner";
import { createCountry, deleteCountry, searchCountries, updateCountry, type CountryDto, type CountryInput } from "@/api/countries";
import { useAuth } from "@/auth/use-auth";
import { CountriesPermissions } from "@/lib/permissions";
import { ApiRequestError, describeError } from "@/lib/api-client";
import { EntityPageHeader, Field, Pagination } from "@/components/list";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Dialog, DialogBody, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

const blank: CountryInput = { name: "", iso2Code: "", iso3Code: "", numericCode: "", phoneCode: "", isActive: true };
const fields = [
  ["name", "País", 150],
  ["iso2Code", "Código ISO 2", 2],
  ["iso3Code", "Código ISO 3", 3],
  ["numericCode", "Código Numérico ISO", 3],
  ["phoneCode", "Código Telefônico", 5],
] as const;

export function CountriesPage() {
  const { user } = useAuth();
  const can = (permission: string) => user?.permissions.includes(permission) ?? false;
  const client = useQueryClient();
  const [search, setSearch] = useState("");
  const [term, setTerm] = useState("");
  const [page, setPage] = useState(1);
  const [active, setActive] = useState("");
  const [sortBy, setSortBy] = useState("name");
  const [sortDir, setSortDir] = useState("asc");
  const [editor, setEditor] = useState<CountryDto | "new" | null>(null);
  const [deleting, setDeleting] = useState<CountryDto | null>(null);
  const [toggling, setToggling] = useState<CountryDto | null>(null);

  useEffect(() => {
    const timer = setTimeout(() => { setTerm(search.trim()); setPage(1); }, 250);
    return () => clearTimeout(timer);
  }, [search]);

  const query = useQuery({
    queryKey: ["countries", { term, page, active, sortBy, sortDir }],
    queryFn: () => searchCountries({ search: term || undefined, pageNumber: page, isActive: active === "" ? undefined : active === "true", sortBy, sortDir }),
    placeholderData: keepPreviousData,
  });
  const refresh = () => client.invalidateQueries({ queryKey: ["countries"] });
  const deletion = useMutation({
    mutationFn: (country: CountryDto) => deleteCountry(country.id),
    onSuccess: () => { toast.success("País excluído."); setDeleting(null); setPage(1); void refresh(); },
    onError: (error) => toast.error("Não foi possível excluir o país.", { description: describeError(error) }),
  });
  const toggle = useMutation({
    mutationFn: (country: CountryDto) => updateCountry(country.id, { ...country, isActive: !country.isActive }),
    onSuccess: () => { toast.success("Situação atualizada."); setToggling(null); void refresh(); },
    onError: (error) => toast.error("Não foi possível alterar a situação.", { description: describeError(error) }),
  });
  const data = query.data;
  const items = data?.items ?? [];
  return (
    <div className="space-y-5">
      <EntityPageHeader icon={Globe} title="Países" total={data?.totalCount} unit="país" unitPlural="países" description="Catálogo global compartilhado por todas as agências.">
        {can(CountriesPermissions.Create) && <Button onClick={() => setEditor("new")}><Plus className="mr-2 h-4 w-4" />Novo país</Button>}
      </EntityPageHeader>
      <div className="flex flex-wrap gap-3">
        <Input aria-label="Pesquisar países" placeholder="Pesquisar por país ou código ISO…" value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-sm" />
        <Select label="Situação" value={active} onChange={(value) => { setActive(value); setPage(1); }} placeholder="Todas" options={[{value:"true", label:"Ativos"}, {value:"false", label:"Inativos"}]} />
        <Select label="Ordenar por" value={sortBy} onChange={(value) => { setSortBy(value); setPage(1); }} options={[{value:"name",label:"País"},{value:"iso2Code",label:"Código ISO 2"},{value:"iso3Code",label:"Código ISO 3"},{value:"numericCode",label:"Código Numérico"},{value:"phoneCode",label:"Código Telefônico"},{value:"isActive",label:"Situação"}]} />
        <Select label="Ordem" value={sortDir} onChange={(value) => { setSortDir(value); setPage(1); }} options={[{value:"asc",label:"Crescente"},{value:"desc",label:"Decrescente"}]} />
      </div>
      {query.isError ? <div role="alert" className="text-[var(--color-destructive)]">Não foi possível carregar os países. {describeError(query.error)} <Button variant="outline" onClick={() => void query.refetch()}>Tentar novamente</Button></div> :
       query.isLoading ? <p role="status">Carregando países…</p> : (
        <div className="rounded-xl border border-[var(--color-border)] bg-[var(--color-card)]">
          <Table>
            <TableHeader><TableRow>{["País","Código ISO 2","Código ISO 3","Código Numérico","Código Telefônico","Situação","Ações"].map(label => <TableHead key={label}>{label}</TableHead>)}</TableRow></TableHeader>
            <TableBody>
              {items.length === 0 && <TableRow><TableCell colSpan={7}>Nenhum país encontrado.</TableCell></TableRow>}
              {items.map(country => <TableRow key={country.id}>
                <TableCell>{country.name}</TableCell><TableCell>{country.iso2Code}</TableCell><TableCell>{country.iso3Code}</TableCell>
                <TableCell>{country.numericCode ?? "—"}</TableCell><TableCell>{country.phoneCode ?? "—"}</TableCell><TableCell>{country.isActive ? "Ativo" : "Inativo"}</TableCell>
                <TableCell><div className="flex gap-2">
                  {can(CountriesPermissions.Update) && <><Button size="sm" variant="outline" aria-label={"Editar " + country.name} onClick={() => setEditor(country)}>Editar</Button>
                    <Button size="sm" variant="outline" onClick={() => setToggling(country)}>{country.isActive ? "Desativar" : "Ativar"}</Button></>}
                  {can(CountriesPermissions.Delete) && <Button size="sm" variant="outline" aria-label={"Excluir " + country.name} onClick={() => setDeleting(country)}>Excluir</Button>}
                </div></TableCell>
              </TableRow>)}
            </TableBody>
          </Table>
        </div>)}
      {data && !query.isError && <Pagination page={page} totalPages={data.totalPages} totalCount={data.totalCount} shown={items.length} fetching={query.isFetching} hasPrev={page > 1} hasNext={page < data.totalPages} onPrev={() => setPage(p => p - 1)} onNext={() => setPage(p => p + 1)} noun="países" locale="pt-BR" />}
      {editor !== null && <CountryEditor key={editor === "new" ? "new" : editor.id} country={editor === "new" ? undefined : editor} onClose={() => setEditor(null)} />}
      <ConfirmDialog open={deleting !== null} onOpenChange={(open) => { if (!open) setDeleting(null); }} title="Excluir país" description={"Deseja excluir " + (deleting?.name ?? "") + "? Países vinculados a outros registros não podem ser excluídos."} confirmLabel="Excluir" cancelLabel="Cancelar" pendingLabel="Excluindo…" pending={deletion.isPending} destructive onConfirm={() => { if (deleting) deletion.mutate(deleting); }} />
      <ConfirmDialog open={toggling !== null} onOpenChange={(open) => { if (!open) setToggling(null); }} title={toggling?.isActive ? "Desativar país" : "Ativar país"} description={"Confirma a alteração da situação de " + (toggling?.name ?? "") + "?"} confirmLabel="Confirmar" cancelLabel="Cancelar" pendingLabel="Salvando…" pending={toggle.isPending} onConfirm={() => { if (toggling) toggle.mutate(toggling); }} />
    </div>
  );
}

function CountryEditor({country, onClose}: {country?: CountryDto; onClose: () => void}) {
  const client = useQueryClient();
  const [values, setValues] = useState<CountryInput>(country ?? blank);
  const [errors, setErrors] = useState<Record<string,string>>({});
  const [failure, setFailure] = useState("");
  const mutation = useMutation({
    mutationFn: (input: CountryInput) => country ? updateCountry(country.id, input) : createCountry(input),
    onSuccess: () => { toast.success(country ? "País atualizado." : "País cadastrado."); void client.invalidateQueries({ queryKey: ["countries"] }); onClose(); },
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
    const input = { ...values, name: values.name.trim(), iso2Code: values.iso2Code.trim().toUpperCase(), iso3Code: values.iso3Code.trim().toUpperCase(), numericCode: values.numericCode?.trim() || null, phoneCode: values.phoneCode?.trim() || null };
    const next: Record<string,string> = {};
    if (!input.name || input.name.length > 150) next.name = "Informe o país com até 150 caracteres.";
    if (!/^[A-Z]{2}$/.test(input.iso2Code)) next.iso2Code = "Informe exatamente 2 letras.";
    if (!/^[A-Z]{3}$/.test(input.iso3Code)) next.iso3Code = "Informe exatamente 3 letras.";
    if (input.numericCode && !/^[0-9]{3}$/.test(input.numericCode)) next.numericCode = "Informe exatamente 3 dígitos.";
    if (input.phoneCode && !/^\+[1-9][0-9]{0,3}$/.test(input.phoneCode)) next.phoneCode = "Informe + e de 1 a 4 dígitos, por exemplo +55.";
    setErrors(next); setFailure("");
    if (!Object.keys(next).length) mutation.mutate(input);
  };
  return <Dialog open onOpenChange={(open) => { if (!open && !mutation.isPending) onClose(); }}>
    <DialogContent><form onSubmit={submit} noValidate>
      <DialogHeader><DialogTitle>{country ? "Editar país" : "Novo país"}</DialogTitle><DialogDescription>Informe os dados do país no catálogo global.</DialogDescription></DialogHeader>
      <DialogBody className="space-y-4">
        {failure && <p role="alert" className="text-[var(--color-destructive)]">{failure}</p>}
        {fields.map(([key,label,max]) => <Field key={key} id={"country-"+key} label={label} required={key==="name" || key==="iso2Code" || key==="iso3Code"} error={errors[key]}>
          <Input id={"country-"+key} value={values[key] ?? ""} maxLength={max} disabled={mutation.isPending} onChange={event => setValues(v => ({...v,[key]:event.target.value}))} autoFocus={key==="name"} />
        </Field>)}
        <Field id="country-active" label="Ativo"><Switch id="country-active" checked={values.isActive} onCheckedChange={(isActive) => setValues(v => ({...v,isActive}))} disabled={mutation.isPending} /></Field>
      </DialogBody>
      <DialogFooter><Button type="button" variant="outline" disabled={mutation.isPending} onClick={onClose}>Cancelar</Button><Button type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Salvando…" : "Salvar"}</Button></DialogFooter>
    </form></DialogContent>
  </Dialog>;
}

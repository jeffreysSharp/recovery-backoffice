import { useEffect, useState, type FormEvent } from "react";
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Globe, Plus } from "lucide-react";
import { toast } from "sonner";
import { createAgencyType, deleteAgencyType, searchAgencyTypes, updateAgencyType, type AgencyTypeDto, type AgencyTypeInput } from "@/api/agency-types";
import { useAuth } from "@/auth/use-auth";
import { AgencyTypesPermissions } from "@/lib/permissions";
import { ApiRequestError, describeError } from "@/lib/api-client";
import { EntityPageHeader, Field, Pagination } from "@/components/list";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Dialog, DialogBody, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

const blank: AgencyTypeInput = { name: "", description: "", isActive: true };


export function AgencyTypesPage() {
  const { user } = useAuth();
  const can = (permission: string) => user?.permissions.includes(permission) ?? false;
  const client = useQueryClient();
  const [search, setSearch] = useState("");
  const [term, setTerm] = useState("");
  const [page, setPage] = useState(1);
  const [active, setActive] = useState("");
  const [sortBy, setSortBy] = useState("name");
  const [sortDir, setSortDir] = useState("asc");
  const [editor, setEditor] = useState<AgencyTypeDto | "new" | null>(null);
  const [deleting, setDeleting] = useState<AgencyTypeDto | null>(null);
  const [toggling, setToggling] = useState<AgencyTypeDto | null>(null);

  useEffect(() => {
    const timer = setTimeout(() => { setTerm(search.trim()); setPage(1); }, 250);
    return () => clearTimeout(timer);
  }, [search]);

  const query = useQuery({
    queryKey: ["agency-types", { term, page, active, sortBy, sortDir }],
    queryFn: () => searchAgencyTypes({ search: term || undefined, pageNumber: page, isActive: active === "" ? undefined : active === "true", sortBy, sortDir }),
    placeholderData: keepPreviousData,
  });
  const refresh = () => client.invalidateQueries({ queryKey: ["agency-types"] });
  const deletion = useMutation({
    mutationFn: (agencyType: AgencyTypeDto) => deleteAgencyType(agencyType.id),
    onSuccess: () => { toast.success("Tipo de Agência excluído."); setDeleting(null); setPage(1); void refresh(); },
    onError: (error) => toast.error("Não foi possível excluir o tipo de agência.", { description: describeError(error) }),
  });
  const toggle = useMutation({
    mutationFn: (agencyType: AgencyTypeDto) => updateAgencyType(agencyType.id, { ...agencyType, isActive: !agencyType.isActive }),
    onSuccess: () => { toast.success("Situação atualizada."); setToggling(null); void refresh(); },
    onError: (error) => toast.error("Não foi possível alterar a situação.", { description: describeError(error) }),
  });
  const data = query.data;
  const items = data?.items ?? [];
  return (
    <div className="space-y-5">
      <EntityPageHeader icon={Globe} title="Tipos de Agência" total={data?.totalCount} unit="tipo de agência" unitPlural="tipos de agência" description="Catálogo global compartilhado por todas as agências.">
        {can(AgencyTypesPermissions.Create) && <Button onClick={() => setEditor("new")}><Plus className="mr-2 h-4 w-4" />Novo tipo de agência</Button>}
      </EntityPageHeader>
      <div className="flex flex-wrap gap-3">
        <Input aria-label="Pesquisar tipos de agência" placeholder="Pesquisar por tipo de agência ou descrição…" value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-sm" />
        <Select label="Situação" value={active} onChange={(value) => { setActive(value); setPage(1); }} placeholder="Todas" options={[{value:"true", label:"Ativos"}, {value:"false", label:"Inativos"}]} />
        <Select label="Ordenar por" value={sortBy} onChange={(value) => { setSortBy(value); setPage(1); }} options={[{value:"name",label:"Tipo de Agência"},{value:"description",label:"Descrição"},{value:"isActive",label:"Situação"}]} />
        <Select label="Ordem" value={sortDir} onChange={(value) => { setSortDir(value); setPage(1); }} options={[{value:"asc",label:"Crescente"},{value:"desc",label:"Decrescente"}]} />
      </div>
      {query.isError ? <div role="alert" className="text-[var(--color-destructive)]">Não foi possível carregar os tipos de agência. {describeError(query.error)} <Button variant="outline" onClick={() => void query.refetch()}>Tentar novamente</Button></div> :
       query.isLoading ? <p role="status">Carregando tipos de agência…</p> : (
        <div className="rounded-xl border border-[var(--color-border)] bg-[var(--color-card)]">
          <Table>
            <TableHeader><TableRow>{["Tipo de Agência","Descrição","Situação","Ações"].map(label => <TableHead key={label}>{label}</TableHead>)}</TableRow></TableHeader>
            <TableBody>
              {items.length === 0 && <TableRow><TableCell colSpan={4}>Nenhum tipo de agência encontrado.</TableCell></TableRow>}
              {items.map(agencyType => <TableRow key={agencyType.id}>
                <TableCell>{agencyType.name}</TableCell><TableCell>{agencyType.description ?? "—"}</TableCell><TableCell>{agencyType.isActive ? "Ativo" : "Inativo"}</TableCell>
                <TableCell><div className="flex gap-2">
                  {can(AgencyTypesPermissions.Update) && <><Button size="sm" variant="outline" aria-label={"Editar " + agencyType.name} onClick={() => setEditor(agencyType)}>Editar</Button>
                    <Button size="sm" variant="outline" onClick={() => setToggling(agencyType)}>{agencyType.isActive ? "Desativar" : "Ativar"}</Button></>}
                  {can(AgencyTypesPermissions.Delete) && <Button size="sm" variant="outline" aria-label={"Excluir " + agencyType.name} onClick={() => setDeleting(agencyType)}>Excluir</Button>}
                </div></TableCell>
              </TableRow>)}
            </TableBody>
          </Table>
        </div>)}
      {data && !query.isError && <Pagination page={page} totalPages={data.totalPages} totalCount={data.totalCount} shown={items.length} fetching={query.isFetching} hasPrev={page > 1} hasNext={page < data.totalPages} onPrev={() => setPage(p => p - 1)} onNext={() => setPage(p => p + 1)} noun="tipos de agência" locale="pt-BR" />}
      {editor !== null && <AgencyTypeEditor key={editor === "new" ? "new" : editor.id} agencyType={editor === "new" ? undefined : editor} onClose={() => setEditor(null)} />}
      <ConfirmDialog open={deleting !== null} onOpenChange={(open) => { if (!open) setDeleting(null); }} title="Excluir tipo de agência" description={"Deseja excluir " + (deleting?.name ?? "") + "? Tipos de Agência vinculados a outros registros não podem ser excluídos."} confirmLabel="Excluir" cancelLabel="Cancelar" pendingLabel="Excluindo…" pending={deletion.isPending} destructive onConfirm={() => { if (deleting) deletion.mutate(deleting); }} />
      <ConfirmDialog open={toggling !== null} onOpenChange={(open) => { if (!open) setToggling(null); }} title={toggling?.isActive ? "Desativar tipo de agência" : "Ativar tipo de agência"} description={"Confirma a alteração da situação de " + (toggling?.name ?? "") + "?"} confirmLabel="Confirmar" cancelLabel="Cancelar" pendingLabel="Salvando…" pending={toggle.isPending} onConfirm={() => { if (toggling) toggle.mutate(toggling); }} />
    </div>
  );
}

function AgencyTypeEditor({agencyType, onClose}: {agencyType?: AgencyTypeDto; onClose: () => void}) {
  const client = useQueryClient();
  const [values, setValues] = useState<AgencyTypeInput>(agencyType ? { name: agencyType.name, description: agencyType.description, isActive: agencyType.isActive } : blank);
  const [errors, setErrors] = useState<Record<string,string>>({});
  const [failure, setFailure] = useState("");
  const mutation = useMutation({
    mutationFn: (input: AgencyTypeInput) => agencyType ? updateAgencyType(agencyType.id, input) : createAgencyType(input),
    onSuccess: () => { toast.success(agencyType ? "Tipo de Agência atualizado." : "Tipo de Agência cadastrado."); void client.invalidateQueries({ queryKey: ["agency-types"] }); onClose(); },
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
    const input = { ...values, name: values.name.trim().normalize("NFC").replace(/\s+/gu, " "), description: values.description?.trim() || null };
    const next: Record<string,string> = {};
    if (input.name.length < 3 || input.name.length > 100) next.name = "Tipo de agência deve ter entre 3 e 100 caracteres.";
    if (input.description && input.description.length > 500) next.description = "Descrição deve ter no máximo 500 caracteres.";
    setErrors(next); setFailure("");
    if (!Object.keys(next).length) mutation.mutate(input);
  };
  return <Dialog open onOpenChange={(open) => { if (!open && !mutation.isPending) onClose(); }}>
    <DialogContent><form onSubmit={submit} noValidate>
      <DialogHeader><DialogTitle>{agencyType ? "Editar tipo de agência" : "Novo tipo de agência"}</DialogTitle><DialogDescription>Informe os dados do tipo de agência no catálogo global.</DialogDescription></DialogHeader>
      <DialogBody className="space-y-4">
        {failure && <p role="alert" className="text-[var(--color-destructive)]">{failure}</p>}
        <Field id="agencyType-name" label="Tipo de Agência" required error={errors.name}>
          <Input id="agencyType-name" value={values.name} maxLength={100} disabled={mutation.isPending} onChange={event => setValues(v => ({ ...v, name: event.target.value }))} autoFocus />
        </Field>
        <Field id="agencyType-description" label="Descrição" error={errors.description}>
          <textarea id="agencyType-description" value={values.description ?? ""} maxLength={500} rows={4} disabled={mutation.isPending} onChange={event => setValues(v => ({ ...v, description: event.target.value }))} className="w-full min-w-0 rounded-lg border border-[var(--color-input)] bg-transparent px-3 py-2 text-sm shadow-xs outline-none placeholder:text-[var(--color-muted-foreground)] focus-visible:border-[var(--color-ring)] focus-visible:ring-[3px] focus-visible:ring-[oklch(from_var(--color-ring)_l_c_h_/_0.5)] disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-[var(--color-destructive)]" />
        </Field>
        <Field id="agencyType-active" label="Ativo"><Switch id="agencyType-active" checked={values.isActive} onCheckedChange={(isActive) => setValues(v => ({...v,isActive}))} disabled={mutation.isPending} /></Field>
      </DialogBody>
      <DialogFooter><Button type="button" variant="outline" disabled={mutation.isPending} onClick={onClose}>Cancelar</Button><Button type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Salvando…" : "Salvar"}</Button></DialogFooter>
    </form></DialogContent>
  </Dialog>;
}

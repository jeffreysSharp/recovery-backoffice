import { apiFetch } from "@/lib/api-client";
import type { PagedResponse } from "@/lib/api-types";
export type AgencyTypeInput = { name: string; description: string | null; isActive: boolean };
export type AgencyTypeDto = AgencyTypeInput & { id: string };
const base = "/api/v1/catalog/agency-types";
export function searchAgencyTypes(params: { search?: string; isActive?: boolean; pageNumber: number; sortBy: string; sortDir: string }) {
  const query = new URLSearchParams({ pageNumber: String(params.pageNumber), pageSize: "20", sortBy: params.sortBy, sortDir: params.sortDir });
  if (params.search) query.set("search", params.search);
  if (params.isActive !== undefined) query.set("isActive", String(params.isActive));
  return apiFetch<PagedResponse<AgencyTypeDto>>(base + "?" + query.toString());
}
export const createAgencyType = (input: AgencyTypeInput) => apiFetch<string>(base, { method: "POST", body: JSON.stringify(input) });
export const updateAgencyType = (id: string, input: AgencyTypeInput) => apiFetch<string>(base + "/" + encodeURIComponent(id), { method: "PUT", body: JSON.stringify({ ...input, agencyTypeId: id }) });
export const deleteAgencyType = (id: string) => apiFetch<void>(base + "/" + encodeURIComponent(id), { method: "DELETE" });

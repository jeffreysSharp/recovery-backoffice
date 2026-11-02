import { apiFetch } from "@/lib/api-client";
import type { PagedResponse } from "@/lib/api-types";
import { searchCountries, type CountryDto } from "@/api/countries";

export type StateInput = { countryId: string; name: string; code: string; ibgeCode: string | null; isActive: boolean };
export type StateDto = StateInput & { id: string; countryName: string };
const base = "/api/v1/catalog/states";

export function searchStates(params: { search?: string; countryId?: string; isActive?: boolean; pageNumber: number; sortBy: string; sortDir: string }) {
  const query = new URLSearchParams({ pageNumber: String(params.pageNumber), pageSize: "20", sortBy: params.sortBy, sortDir: params.sortDir });
  if (params.search) query.set("search", params.search);
  if (params.countryId) query.set("countryId", params.countryId);
  if (params.isActive !== undefined) query.set("isActive", String(params.isActive));
  return apiFetch<PagedResponse<StateDto>>(base + "?" + query.toString());
}
export const createState = (input: StateInput) => apiFetch<string>(base, { method: "POST", body: JSON.stringify(input) });
export const updateState = (id: string, input: StateInput) => apiFetch<string>(base + "/" + encodeURIComponent(id), { method: "PUT", body: JSON.stringify({ ...input, stateId: id }) });
export const deleteState = (id: string) => apiFetch<void>(base + "/" + encodeURIComponent(id), { method: "DELETE" });

export async function loadStateCountries(): Promise<CountryDto[]> {
  const countries: CountryDto[] = [];
  let pageNumber = 1;
  while (true) {
    const page = await searchCountries({ pageNumber, sortBy: "name", sortDir: "asc" });
    countries.push(...page.items);
    if (pageNumber >= page.totalPages) return countries;
    pageNumber++;
  }
}

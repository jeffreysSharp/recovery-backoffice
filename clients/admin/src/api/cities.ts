import { apiFetch } from "@/lib/api-client";
import type { PagedResponse } from "@/lib/api-types";
import { searchStates, type StateDto } from "@/api/states";
export { loadStateCountries as loadCityCountries } from "@/api/states";
export type CityInput = { stateId: string; name: string; ibgeCode: string | null; isActive: boolean };
export type CityDto = CityInput & { id: string; stateName: string; countryId: string; countryName: string };
const base = "/api/v1/catalog/cities";
export function searchCities(params: { search?: string; countryId?: string; stateId?: string; isActive?: boolean; pageNumber: number; sortBy: string; sortDir: string }) {
  const query = new URLSearchParams({ pageNumber: String(params.pageNumber), pageSize: "20", sortBy: params.sortBy, sortDir: params.sortDir });
  if (params.search) query.set("search", params.search);
  if (params.countryId) query.set("countryId", params.countryId);
  if (params.stateId) query.set("stateId", params.stateId);
  if (params.isActive !== undefined) query.set("isActive", String(params.isActive));
  return apiFetch<PagedResponse<CityDto>>(base + "?" + query.toString());
}
export const createCity = (input: CityInput) => apiFetch<string>(base, { method: "POST", body: JSON.stringify(input) });
export const updateCity = (id: string, input: CityInput) => apiFetch<string>(base + "/" + encodeURIComponent(id), { method: "PUT", body: JSON.stringify({ ...input, cityId: id }) });
export const deleteCity = (id: string) => apiFetch<void>(base + "/" + encodeURIComponent(id), { method: "DELETE" });
export async function loadCityStates(countryId?: string): Promise<StateDto[]> {
  const states: StateDto[] = [];
  let pageNumber = 1;
  while (true) {
    const page = await searchStates({ countryId, pageNumber, sortBy: "name", sortDir: "asc" });
    states.push(...page.items);
    if (pageNumber >= page.totalPages) return states;
    pageNumber++;
  }
}

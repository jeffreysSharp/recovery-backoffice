import { apiFetch } from "@/lib/api-client";
import type { PagedResponse } from "@/lib/api-types";

export type CountryInput = {
  name: string;
  iso2Code: string;
  iso3Code: string;
  numericCode: string | null;
  phoneCode: string | null;
  isActive: boolean;
};
export type CountryDto = CountryInput & { id: string };
const base = "/api/v1/catalog/countries";

export function searchCountries(params: { search?: string; isActive?: boolean; pageNumber: number; sortBy: string; sortDir: string }) {
  const query = new URLSearchParams({ pageNumber: String(params.pageNumber), pageSize: "20", sortBy: params.sortBy, sortDir: params.sortDir });
  if (params.search) query.set("search", params.search);
  if (params.isActive !== undefined) query.set("isActive", String(params.isActive));
  return apiFetch<PagedResponse<CountryDto>>(base + "?" + query.toString());
}
export const createCountry = (input: CountryInput) => apiFetch<string>(base, { method: "POST", body: JSON.stringify(input) });
export const updateCountry = (id: string, input: CountryInput) => apiFetch<string>(base + "/" + encodeURIComponent(id), { method: "PUT", body: JSON.stringify({ ...input, countryId: id }) });
export const deleteCountry = (id: string) => apiFetch<void>(base + "/" + encodeURIComponent(id), { method: "DELETE" });

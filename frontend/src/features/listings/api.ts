import client from "@/shared/api/client";
import type { ListingDto, PagedResult, CreateListingRequest } from "./types";

export const listingsApi = {
  getListings: (pageNumber = 1, pageSize = 10, searchTerm?: string) =>
    client.get<PagedResult<ListingDto>>("/api/v1/listings", {
      params: { pageNumber, pageSize, searchTerm },
    }),

  getListing: (id: string) => client.get<ListingDto>(`/api/v1/listings/${id}`),

  createListing: (data: CreateListingRequest) =>
    client.post<string>("/api/v1/listings", data),

  updateListing: (id: string, data: CreateListingRequest) =>
    client.put(`/api/v1/listings/${id}`, data),

  cancelListing: (id: string) => client.post(`/api/v1/listings/${id}/cancel`),

  getMyListings: (pageNumber = 1, pageSize = 10, statuses?: string[]) =>
    client.get<PagedResult<ListingDto>>("/api/v1/listings/mine", {
      params: { pageNumber, pageSize, statuses },
    }),
};

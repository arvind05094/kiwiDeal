import client from "@/shared/api/client";
import type {
  ListingDto,
  PagedResult,
  WatchlistItemDto,
} from "@/shared/types/common";
import type {
  GetListingsParams,
  GetMyListingsParams,
  CreateListingRequest,
  UpdateListingRequest,
} from "./types";

export const listingsApi = {
  getListings: async (
    params: GetListingsParams,
  ): Promise<PagedResult<ListingDto>> => {
    const res = await client.get("/listings", { params });
    return res.data;
  },

  getListing: async (id: string): Promise<ListingDto> => {
    const res = await client.get(`/listings/${id}`);
    return res.data;
  },

  getMyListings: async (
    params: GetMyListingsParams,
  ): Promise<PagedResult<ListingDto>> => {
    const res = await client.get("/listings/mine", { params });
    return res.data;
  },

  createListing: async (data: CreateListingRequest): Promise<string> => {
    const res = await client.post("/listings", data);
    return res.data;
  },

  updateListing: async (
    id: string,
    data: UpdateListingRequest,
  ): Promise<void> => {
    await client.put(`/listings/${id}`, data);
  },

  cancelListing: async (id: string): Promise<void> => {
    await client.post(`/listings/${id}/cancel`);
  },

  uploadImages: async (
    id: string,
    files: File[],
  ): Promise<{ imageUrls: string[] }> => {
    const form = new FormData();
    files.forEach((f) => form.append("files", f));
    const res = await client.post(`/listings/${id}/images`, form, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return res.data;
  },

  getWatchlist: async (params: {
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PagedResult<WatchlistItemDto>> => {
    const res = await client.get("/listings/watchlist", { params });
    return res.data;
  },

  addToWatchlist: async (id: string): Promise<void> => {
    await client.post(`/listings/${id}/watchlist`);
  },

  removeFromWatchlist: async (id: string): Promise<void> => {
    await client.delete(`/listings/${id}/watchlist`);
  },

  isWatched: async (id: string): Promise<boolean> => {
    const res = await client.get(`/listings/${id}/watchlist`);
    return res.data.isWatched;
  },
};

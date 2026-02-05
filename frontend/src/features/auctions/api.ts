import client from "@/shared/api/client";
import type { AuctionDto, PagedResult } from "@/shared/types/common";
import type {
  GetAuctionsParams,
  CreateAuctionRequest,
  PlaceBidRequest,
} from "./types";

export const auctionsApi = {
  getAuctions: async (
    params: GetAuctionsParams,
  ): Promise<PagedResult<AuctionDto>> => {
    const res = await client.get("/auctions", { params });
    return res.data;
  },

  getAuction: async (id: string): Promise<AuctionDto> => {
    const res = await client.get(`/auctions/${id}`);
    return res.data;
  },

  createAuction: async (data: CreateAuctionRequest): Promise<string> => {
    const res = await client.post("/auctions", data);
    return res.data;
  },

  placeBid: async (id: string, data: PlaceBidRequest): Promise<void> => {
    await client.post(`/auctions/${id}/bids`, data);
  },

  closeAuction: async (id: string): Promise<void> => {
    await client.post(`/auctions/${id}/close`);
  },

  addToWatchlist: async (id: string): Promise<void> => {
    await client.post(`/auctions/${id}/watchlist`);
  },

  removeFromWatchlist: async (id: string): Promise<void> => {
    await client.delete(`/auctions/${id}/watchlist`);
  },
};

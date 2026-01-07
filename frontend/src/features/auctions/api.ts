import client from "@/shared/api/client";
import type {
  AuctionDto,
  PagedResult,
  CreateAuctionRequest,
  PlaceBidRequest,
} from "./types";

export const auctionsApi = {
  getAuctions: (pageNumber = 1, pageSize = 10) =>
    client.get<PagedResult<AuctionDto>>("/api/v1/auctions", {
      params: { pageNumber, pageSize },
    }),

  getAuction: (id: string) => client.get<AuctionDto>(`/api/v1/auctions/${id}`),

  createAuction: (data: CreateAuctionRequest) =>
    client.post<string>("/api/v1/auctions", data),

  placeBid: (id: string, data: PlaceBidRequest) =>
    client.post(`/api/v1/auctions/${id}/bids`, data),

  closeAuction: (id: string) => client.post(`/api/v1/auctions/${id}/close`),
};

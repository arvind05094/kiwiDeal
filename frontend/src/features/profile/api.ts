import client from "@/shared/api/client";
import type {
  UserProfileDto,
  MyProfileDto,
  PagedResult,
  ListingDto,
  AuctionDto,
  WatchlistItemDto,
} from "@/shared/types/common";
import type {
  UpdateProfileRequest,
  GetMyListingsParams,
  GetMyAuctionsParams,
  GetBuyingParams,
  AccountOverviewDto,
} from "./types";

export const profileApi = {
  getPublicProfile: async (id: string): Promise<UserProfileDto> => {
    const res = await client.get(`/users/${id}`);
    return res.data;
  },

  getMyProfile: async (): Promise<MyProfileDto> => {
    const res = await client.get("/users/me");
    return res.data;
  },

  updateProfile: async (data: UpdateProfileRequest): Promise<void> => {
    await client.put("/users/me", data);
  },

  getMyListings: async (
    params: GetMyListingsParams,
  ): Promise<PagedResult<ListingDto>> => {
    const res = await client.get("/listings/mine", { params });
    return res.data;
  },

  getMySelling: async (
    params: GetMyAuctionsParams,
  ): Promise<PagedResult<AuctionDto>> => {
    const res = await client.get("/auctions/mine", { params });
    return res.data;
  },

  getMyBuying: async (
    params: GetBuyingParams,
  ): Promise<PagedResult<AuctionDto>> => {
    const res = await client.get("/auctions/bidding", { params });
    return res.data;
  },

  getFixedPriceSales: async (params: {
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PagedResult<ListingDto>> => {
    const res = await client.get("/payments/sales", { params });
    return res.data;
  },

  getFixedPricePurchases: async (params: {
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PagedResult<ListingDto>> => {
    const res = await client.get("/payments/purchases", { params });
    return res.data;
  },

  getListingWatchlist: async (params: {
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PagedResult<WatchlistItemDto>> => {
    const res = await client.get("/listings/watchlist", { params });
    return res.data;
  },

  getAuctionWatchlist: async (params: {
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PagedResult<AuctionDto>> => {
    const res = await client.get("/auctions/watchlist", { params });
    return res.data;
  },
};

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  region: string;
}

export interface GetMyListingsParams {
  statuses?: string[];
  pageNumber?: number;
  pageSize?: number;
}

export interface GetMyAuctionsParams {
  status?: "Scheduled" | "Active" | "Sold" | "Unsold";
  pageNumber?: number;
  pageSize?: number;
}

export interface GetBuyingParams {
  status?: "Active" | "Won" | "Lost";
  pageNumber?: number;
  pageSize?: number;
}

export interface AccountOverviewDto {
  totalActiveListings: number;
  totalActiveAuctions: number;
  totalRevenue: number;
  totalSales: number;
}

export interface UserRatingDto {
  stars: number;
  comment: string | null;
  raterName: string;
  createdAt: string;
}

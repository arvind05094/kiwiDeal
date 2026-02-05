export interface GetAuctionsParams {
  pageNumber?: number;
  pageSize?: number;
  endingSoon?: boolean;
}

export interface CreateAuctionRequest {
  listingId: string;
  listingTitle: string;
  startingPrice: number;
  startTime: string;
  endTime: string;
}

export interface PlaceBidRequest {
  amount: number;
}

export interface BidDto {
  id: string;
  bidderId: string;
  amount: number;
  createdAt: string;
}

export interface AuctionDto {
  id: string;
  listingId: string;
  sellerId: string;
  startingPrice: number;
  currentHighestBid: number | null;
  currentHighestBidderId: string | null;
  startTime: string;
  endTime: string;
  status: "Scheduled" | "Active" | "Closed";
  bids: BidDto[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface CreateAuctionRequest {
  listingId: string;
  startingPrice: number;
  startTime: string;
  endTime: string;
}

export interface PlaceBidRequest {
  amount: number;
}

export interface BidPlacedEvent {
  auctionId: string;
  bidderId: string;
  amount: number;
  newEndTime: string;
}

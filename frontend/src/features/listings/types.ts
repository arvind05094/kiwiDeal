export interface GetListingsParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  category?: string;
  region?: string;
  sortBy?: "newest" | "price_asc" | "price_desc" | "ending_soon";
  listingType?: "Auction" | "FixedPrice";
  sellerId?: string;
}

export interface GetMyListingsParams {
  pageNumber?: number;
  pageSize?: number;
  statuses?: string[];
}

export interface CreateListingRequest {
  title: string;
  description: string;
  listingType: "Auction" | "FixedPrice";
  buyNowPrice?: number;
  category: string;
  region: string;
}

export interface UpdateListingRequest {
  title: string;
  description: string;
}

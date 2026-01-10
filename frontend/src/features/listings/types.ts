export interface ListingImage {
  url: string;
  displayOrder: number;
}

export interface ListingDto {
  id: string;
  sellerId: string;
  title: string;
  description: string;
  startingPrice: number;
  status: "Active" | "Cancelled" | "Sold";
  createdAt: string;
  updatedAt: string | null;
  images: ListingImage[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface CreateListingRequest {
  title: string;
  description: string;
  startingPrice: number;
}

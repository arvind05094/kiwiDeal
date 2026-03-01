export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ProblemDetails {
  title: string;
  status: number;
  detail?: string;
  errors?: Record<string, string[]>;
}

export type ListingType = "Auction" | "FixedPrice";
export type ListingStatus = "Active" | "Sold" | "Cancelled";
export type AuctionStatus = "Scheduled" | "Active" | "Closed";
export type PaymentType = "Auction" | "FixedPrice";
export type PaymentStatus = "Pending" | "Completed" | "Failed";

export interface ListingImageDto {
  url: string;
  displayOrder: number;
}

export interface ListingDto {
  id: string;
  sellerId: string;
  sellerName: string;
  title: string;
  description: string;
  listingType: ListingType;
  buyNowPrice?: number;
  category: string;
  region: string;
  status: ListingStatus;
  createdAt: string;
  updatedAt: string;
  images: ListingImageDto[];
  auctionId?: string; // add this
}

export interface WatchlistItemDto {
  listingId: string;
  title: string;
  status: ListingStatus;
  listingType: ListingType;
  buyNowPrice?: number;
  thumbnailUrl?: string;
  watchedSince: string;
}

export interface AuctionBidDto {
  id: string;
  bidderId: string;
  bidderName: string;
  amount: number;
  createdAt: string;
}

export interface AuctionDto {
  id: string;
  listingId: string;
  listingTitle: string;
  sellerId: string;
  startingPrice: number;
  currentHighestBid?: number;
  currentHighestBidderId?: string;
  startTime: string;
  endTime: string;
  status: AuctionStatus;
  bids: AuctionBidDto[];
}

export interface UserResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  region: string;
}

export interface UserProfileDto {
  id: string;
  firstName: string;
  lastName: string;
  region: string;
  memberSince: string;
  averageRating?: number;
  totalRatings: number;
}

export interface MyProfileDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  region: string;
  memberSince: string;
}
export interface ConversationDto {
  id: string;
  otherUserId: string;
  otherUserName: string;
  lastMessagePreview: string;
  unreadCount: number;
  updatedAt: string;
}

export interface MessageDto {
  id: string;
  conversationId: string;
  senderId: string;
  senderName: string;
  content: string;
  createdAt: string;
}

export interface PaymentDto {
  id: string;
  auctionId?: string;
  listingId: string;
  buyerId: string;
  sellerId: string;
  amount: number;
  paymentType: PaymentType;
  status: PaymentStatus;
  stripeSessionId?: string;
  createdAt: string;
  paidAt?: string;
}

export const NZ_REGIONS = [
  { label: "Northland", value: "Northland" },
  { label: "Auckland", value: "Auckland" },
  { label: "Waikato", value: "Waikato" },
  { label: "Bay of Plenty", value: "BayOfPlenty" },
  { label: "Gisborne", value: "Gisborne" },
  { label: "Hawke's Bay", value: "HawkesBay" },
  { label: "Taranaki", value: "Taranaki" },
  { label: "Manawatū-Whanganui", value: "ManawatuWhanganui" },
  { label: "Wellington", value: "Wellington" },
  { label: "Tasman", value: "Tasman" },
  { label: "Nelson", value: "Nelson" },
  { label: "Marlborough", value: "Marlborough" },
  { label: "West Coast", value: "WestCoast" },
  { label: "Canterbury", value: "Canterbury" },
  { label: "Otago", value: "Otago" },
  { label: "Southland", value: "Southland" },
] as const;

export type NzRegionValue = (typeof NZ_REGIONS)[number]["value"];
export const CATEGORIES = [
  "Electronics",
  "Clothing",
  "Motors",
  "Cars",
  "Furniture",
  "Home and Garden",
  "Sports",
  "Books",
  "Toys",
  "Music",
  "Collectibles",
  "Other",
] as const;

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface ProblemDetails {
  title: string
  status: number
  detail?: string
  errors?: Record<string, string[]>
}

export type ListingType = 'Auction' | 'FixedPrice'
export type ListingStatus = 'Active' | 'Sold' | 'Cancelled'
export type AuctionStatus = 'Scheduled' | 'Active' | 'Closed'
export type PaymentType = 'Auction' | 'FixedPrice'
export type PaymentStatus = 'Pending' | 'Completed' | 'Failed'

export interface ListingImageDto {
  url: string
  displayOrder: number
}

export interface ListingDto {
  id: string
  sellerId: string
  title: string
  description: string
  listingType: ListingType
  buyNowPrice?: number
  category: string
  region: string
  status: ListingStatus
  createdAt: string
  updatedAt: string
  images: ListingImageDto[]
}

export interface WatchlistItemDto {
  listingId: string
  title: string
  status: ListingStatus
  listingType: ListingType
  buyNowPrice?: number
  thumbnailUrl?: string
  watchedSince: string
}

export interface AuctionBidDto {
  id: string
  bidderId: string
  bidderName: string
  amount: number
  createdAt: string
}

export interface AuctionDto {
  id: string
  listingId: string
  listingTitle: string
  sellerId: string
  startingPrice: number
  currentHighestBid?: number
  currentHighestBidderId?: string
  startTime: string
  endTime: string
  status: AuctionStatus
  bids: AuctionBidDto[]
}

export interface UserResponse {
  id: string
  email: string
  firstName: string
  lastName: string
  region: string
}

export interface UserProfileDto {
  id: string
  firstName: string
  lastName: string
  region: string
  memberSince: string
  averageRating?: number
  totalRatings: number
}

export interface MyProfileDto {
  id: string
  email: string
  firstName: string
  lastName: string
  region: string
  memberSince: string
}

export interface ConversationDto {
  id: string
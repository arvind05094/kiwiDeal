export interface CreateAuctionCheckoutRequest {
  auctionId: string;
}

export interface CreateBuyNowCheckoutRequest {
  listingId: string;
  sellerId: string;
  amount: number;
}

export interface CheckoutResponse {
  checkoutUrl: string;
}

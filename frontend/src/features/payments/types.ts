export interface PaymentDto {
  id: string;
  auctionId: string;
  winnerId: string;
  sellerId: string;
  amount: number;
  status: "Pending" | "Completed" | "Failed";
  stripeSessionId: string | null;
  createdAt: string;
  paidAt: string | null;
}

export interface CheckoutRequest {
  auctionId: string;
}

export interface CheckoutResponse {
  checkoutUrl: string;
}

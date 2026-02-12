import client from "@/shared/api/client";
import type { PaymentDto } from "@/shared/types/common";
import type {
  CreateAuctionCheckoutRequest,
  CreateBuyNowCheckoutRequest,
  CheckoutResponse,
} from "./types";

export const createAuctionCheckout = async (
  body: CreateAuctionCheckoutRequest,
): Promise<CheckoutResponse> => {
  const res = await client.post<CheckoutResponse>("/payments/checkout", body);
  return res.data;
};

export const createBuyNowCheckout = async (
  body: CreateBuyNowCheckoutRequest,
): Promise<CheckoutResponse> => {
  const res = await client.post<CheckoutResponse>("/payments/buynow", body);
  return res.data;
};

export const getPaymentByAuction = async (
  auctionId: string,
): Promise<PaymentDto> => {
  const res = await client.get<PaymentDto>(`/payments/${auctionId}`);
  return res.data;
};

export const getPaymentByListing = async (
  listingId: string,
): Promise<PaymentDto> => {
  const res = await client.get<PaymentDto>(`/payments/listing/${listingId}`);
  return res.data;
};

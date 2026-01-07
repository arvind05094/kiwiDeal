import client from "@/shared/api/client";
import type { CheckoutRequest, CheckoutResponse, PaymentDto } from "./types";

export const paymentsApi = {
  checkout: (data: CheckoutRequest) =>
    client.post<CheckoutResponse>("/api/v1/payments/checkout", data),

  getPayment: (auctionId: string) =>
    client.get<PaymentDto>(`/api/v1/payments/${auctionId}`),
};

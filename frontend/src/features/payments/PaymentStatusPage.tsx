import { useParams, useSearchParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { paymentsApi } from "./api";
import Navbar from "@/shared/components/Navbar";

export default function PaymentStatusPage() {
  const { auctionId } = useParams<{ auctionId: string }>();
  const [searchParams] = useSearchParams();
  const success = searchParams.get("success") === "true";

  const {
    data: payment,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["payment", auctionId],
    queryFn: () => paymentsApi.getPayment(auctionId!).then((r) => r.data),
    enabled: !!auctionId,
    refetchInterval: (query) =>
      query.state.data?.status === "Pending" ? 3000 : false,
  });

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <div className="max-w-md mx-auto p-6 mt-10">
        <div className="bg-white rounded-lg shadow p-6 space-y-4">
          <h1 className="text-xl font-bold">Payment</h1>

          {success && (
            <div className="bg-green-50 border border-green-200 text-green-700 rounded p-3 text-sm">
              Payment submitted — confirming with Stripe…
            </div>
          )}

          {isLoading && (
            <p className="text-gray-500 text-sm">Loading payment status…</p>
          )}

          {isError && (
            <p className="text-red-500 text-sm">
              Could not load payment details.
            </p>
          )}

          {payment && (
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-500">Status</span>
                <span
                  className={`font-semibold ${
                    payment.status === "Completed"
                      ? "text-green-600"
                      : payment.status === "Failed"
                        ? "text-red-600"
                        : "text-yellow-600"
                  }`}
                >
                  {payment.status}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-500">Amount</span>
                <span className="font-semibold">
                  ${payment.amount.toFixed(2)}
                </span>
              </div>
              {payment.paidAt && (
                <div className="flex justify-between">
                  <span className="text-gray-500">Paid at</span>
                  <span>{new Date(payment.paidAt).toLocaleString()}</span>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

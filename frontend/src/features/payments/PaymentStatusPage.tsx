import { useParams, useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { CheckCircle, XCircle, Clock } from "lucide-react";
import { getPaymentByAuction } from "./api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { Button } from "@/components/ui/button";

export function PaymentStatusPage() {
  const { auctionId } = useParams<{ auctionId: string }>();
  const navigate = useNavigate();

  const { data: payment, isLoading } = useQuery({
    queryKey: ["payment", auctionId],
    queryFn: () => getPaymentByAuction(auctionId!),
    enabled: !!auctionId,
    refetchInterval: (data) => (data?.status === "Pending" ? 3000 : false),
  });

  if (isLoading) return <LoadingSpinner />;

  if (!payment) {
    return (
      <div className="text-center py-24 text-gray-400">Payment not found.</div>
    );
  }

  return (
    <div className="max-w-md mx-auto py-16 text-center space-y-6">
      {payment.status === "Completed" && (
        <>
          <CheckCircle className="h-16 w-16 text-green-500 mx-auto" />
          <h1 className="text-2xl font-semibold text-gray-900">
            Payment successful
          </h1>
          <p className="text-gray-500">
            You paid{" "}
            <span className="font-medium">${payment.amount.toFixed(2)}</span>.
            The seller will be in touch.
          </p>
        </>
      )}

      {payment.status === "Pending" && (
        <>
          <Clock className="h-16 w-16 text-yellow-500 mx-auto animate-pulse" />
          <h1 className="text-2xl font-semibold text-gray-900">
            Awaiting payment
          </h1>
          <p className="text-gray-500">
            Complete your payment on the Stripe page. This will update
            automatically.
          </p>
        </>
      )}

      {payment.status === "Failed" && (
        <>
          <XCircle className="h-16 w-16 text-red-500 mx-auto" />
          <h1 className="text-2xl font-semibold text-gray-900">
            Payment failed
          </h1>
          <p className="text-gray-500">
            Something went wrong. Please contact the seller to resolve.
          </p>
        </>
      )}

      <Button
        onClick={() => navigate("/account")}
        className="bg-gray-900 hover:bg-gray-800 text-white"
      >
        Go to My Account
      </Button>
    </div>
  );
}

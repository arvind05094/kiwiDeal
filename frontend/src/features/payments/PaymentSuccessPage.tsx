import { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { CheckCircle, Clock } from "lucide-react";
import { Button } from "@/components/ui/button";
import { getPaymentByListing } from "./api";

export function PaymentSuccessPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const paymentId = searchParams.get("paymentId");
  const listingId = searchParams.get("listingId");
  const queryClient = useQueryClient();

  const { data: payment } = useQuery({
    queryKey: ["payment-listing", listingId],
    queryFn: () => getPaymentByListing(listingId!),
    enabled: !!listingId,
    refetchInterval: (query) =>
      query.state.data?.status === "Completed" ? false : 3000,
  });

  const isConfirmed = !listingId || payment?.status === "Completed";

  useEffect(() => {
    if (payment?.status === "Completed") {
      queryClient.invalidateQueries({ queryKey: ["listing", listingId] });
      queryClient.invalidateQueries({ queryKey: ["payments", "purchases"] });
    }
  }, [payment?.status, listingId, queryClient]);

  return (
    <div className="max-w-md mx-auto py-16 text-center space-y-6">
      {isConfirmed ? (
        <CheckCircle className="h-16 w-16 text-green-500 mx-auto" />
      ) : (
        <Clock className="h-16 w-16 text-yellow-500 mx-auto animate-pulse" />
      )}
      <h1 className="text-2xl font-semibold text-gray-900">
        {isConfirmed ? "Payment successful" : "Confirming payment…"}
      </h1>
      <p className="text-gray-500">
        {isConfirmed
          ? "Your payment has been processed successfully. The seller will be in touch."
          : "Please wait while we confirm your payment."}
      </p>
      {paymentId && (
        <p className="text-xs text-gray-400">Payment ID: {paymentId}</p>
      )}
      <Button
        onClick={() => navigate("/account")}
        className="bg-gray-900 hover:bg-gray-800 text-white"
        disabled={!isConfirmed}
      >
        Go to My Account
      </Button>
    </div>
  );
}

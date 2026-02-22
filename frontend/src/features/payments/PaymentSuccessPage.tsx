import { useNavigate, useSearchParams } from "react-router-dom";
import { CheckCircle } from "lucide-react";
import { Button } from "@/components/ui/button";

export function PaymentSuccessPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const paymentId = searchParams.get("paymentId");

  return (
    <div className="max-w-md mx-auto py-16 text-center space-y-6">
      <CheckCircle className="h-16 w-16 text-green-500 mx-auto" />
      <h1 className="text-2xl font-semibold text-gray-900">
        Payment successful
      </h1>
      <p className="text-gray-500">
        Your payment has been processed successfully. The seller will be in
        touch.
      </p>
      {paymentId && (
        <p className="text-xs text-gray-400">Payment ID: {paymentId}</p>
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

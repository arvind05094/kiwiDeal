import { useNavigate } from "react-router-dom";
import { XCircle } from "lucide-react";
import { Button } from "@/components/ui/button";

export function PaymentCancelPage() {
  const navigate = useNavigate();

  return (
    <div className="max-w-md mx-auto py-16 text-center space-y-6">
      <XCircle className="h-16 w-16 text-red-500 mx-auto" />
      <h1 className="text-2xl font-semibold text-gray-900">
        Payment cancelled
      </h1>
      <p className="text-gray-500">
        Your payment was not completed. The listing is still available.
      </p>
      <Button
        onClick={() => navigate("/listings")}
        className="bg-gray-900 hover:bg-gray-800 text-white"
      >
        Back to Listings
      </Button>
    </div>
  );
}

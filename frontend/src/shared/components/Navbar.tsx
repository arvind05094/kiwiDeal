import { useNavigate } from "react-router-dom";
import { useAuth } from "@/features/auth/AuthContext";
import { Button } from "@/components/ui/button";

export default function Navbar() {
  const navigate = useNavigate();
  const { user, setUser } = useAuth();

  const handleLogout = () => {
    setUser(null);
    navigate("/login");
  };

  return (
    <nav className="bg-orange-500 text-white px-6 py-3 flex items-center justify-between shadow">
      <span
        className="text-2xl font-bold tracking-tight cursor-pointer"
        onClick={() => navigate("/listings")}
      >
        kiwiDeal
      </span>
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="sm"
          className="text-white hover:bg-orange-600"
          onClick={() => navigate("/listings")}
        >
          Listings
        </Button>
        <Button
          variant="ghost"
          size="sm"
          className="text-white hover:bg-orange-600"
          onClick={() => navigate("/auctions")}
        >
          Auctions
        </Button>
        {user ? (
          <>
            <span className="text-sm border-l border-orange-300 pl-4">
              Hi, {user.firstName}
            </span>
            <Button
              variant="ghost"
              size="sm"
              className="text-white hover:bg-orange-600"
              onClick={() => navigate("/my-listings")}
            >
              My Listings
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="text-white border-white bg-transparent hover:bg-orange-600 hover:text-white"
              onClick={handleLogout}
            >
              Logout
            </Button>
          </>
        ) : (
          <>
            <Button
              variant="ghost"
              size="sm"
              className="text-white hover:bg-orange-600"
              onClick={() => navigate("/login")}
            >
              Login
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="text-white border-white bg-transparent hover:bg-orange-600 hover:text-white"
              onClick={() => navigate("/register")}
            >
              Register
            </Button>
          </>
        )}
      </div>
    </nav>
  );
}

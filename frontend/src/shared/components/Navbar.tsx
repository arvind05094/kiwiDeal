import { Link, useNavigate } from "react-router-dom";
import { Search, Heart, Inbox, ChevronDown, LogOut, User } from "lucide-react";
import { useState } from "react";
import { useAuth } from "@/features/auth/AuthContext";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

export function Navbar() {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();
  const [search, setSearch] = useState("");

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (search.trim()) {
      navigate(`/listings?searchTerm=${encodeURIComponent(search.trim())}`);
    } else {
      navigate("/listings");
    }
  };

  const handleLogout = async () => {
    await logout();
    navigate("/listings");
  };

  const initials = user
    ? `${user.firstName[0]}${user.lastName[0]}`.toUpperCase()
    : "";

  return (
    <nav className="sticky top-0 z-50 border-b border-gray-200 bg-white">
      <div className="mx-auto flex h-16 max-w-7xl items-center gap-4 px-4">
        {/* Logo */}
        <Link
          to="/listings"
          className="flex-shrink-0 text-xl font-bold text-orange-500"
        >
          kiwiDeal
        </Link>

        {/* Search */}
        <form
          onSubmit={handleSearch}
          className="flex flex-1 items-center gap-2"
        >
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search listings..."
              className="h-10 w-full rounded-lg border border-gray-200 bg-gray-50 pl-9 pr-4 text-sm outline-none transition-colors focus:border-orange-400 focus:bg-white"
            />
          </div>
          <Button
            type="submit"
            className="bg-gray-900 hover:bg-gray-700 text-white"
          >
            Search
          </Button>
        </form>

        {/* Right side */}
        <div className="flex items-center gap-2">
          {isAuthenticated ? (
            <>
              <Link
                to="/watchlist"
                className="flex items-center gap-1.5 rounded-lg px-3 py-2 text-sm text-gray-600 hover:bg-gray-50 hover:text-gray-900 transition-colors"
              >
                <Heart className="h-4 w-4" />
                <span className="hidden sm:inline">Watchlist</span>
              </Link>

              <Link
                to="/messages"
                className="flex items-center gap-1.5 rounded-lg px-3 py-2 text-sm text-gray-600 hover:bg-gray-50 hover:text-gray-900 transition-colors"
              >
                <Inbox className="h-4 w-4" />
                <span className="hidden sm:inline">Inbox</span>
              </Link>

              <Button
                onClick={() => navigate("/listings/new")}
                className="bg-orange-500 hover:bg-orange-600 text-white active:scale-95 transition-all duration-150"
              >
                Post Listing
              </Button>

              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <button className="flex items-center gap-1.5 rounded-lg px-2 py-1.5 text-sm text-gray-700 hover:bg-gray-50 transition-colors">
                    <div className="flex h-8 w-8 items-center justify-center rounded-full bg-gray-200 text-xs font-semibold text-gray-600">
                      {initials}
                    </div>
                    <ChevronDown className="h-3.5 w-3.5 text-gray-400" />
                  </button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-48">
                  <DropdownMenuItem onClick={() => navigate("/account")}>
                    <User className="mr-2 h-4 w-4" />
                    My Account
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    onClick={handleLogout}
                    className="text-red-600 focus:text-red-600"
                  >
                    <LogOut className="mr-2 h-4 w-4" />
                    Logout
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </>
          ) : (
            <>
              <Button
                variant="outline"
                onClick={() => navigate("/login")}
                className="border-gray-200 text-gray-700 hover:bg-gray-50"
              >
                Login
              </Button>
              <Button
                onClick={() => navigate("/register")}
                className="bg-orange-500 hover:bg-orange-600 text-white"
              >
                Register
              </Button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}

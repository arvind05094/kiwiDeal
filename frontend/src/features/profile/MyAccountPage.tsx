import { useState } from "react";
import { OverviewTab } from "./tabs/OverviewTab";
import { ProfileTab } from "./tabs/ProfileTab";
import { MyListingsTab } from "./tabs/MyListingsTab";
import { SellingTab } from "./tabs/SellingTab";
import { BuyingTab } from "./tabs/BuyingTab";
import { FixedPriceSalesTab } from "./tabs/FixedPriceSalesTab";
import { FixedPricePurchasesTab } from "./tabs/FixedPricePurchasesTab";
import { WatchlistTab } from "./tabs/WatchlistTab";
import { cn } from "@/lib/utils";
import { InboxPage } from "../messages/InboxPage";
import { RatingsTab } from "./tabs/RatingsTab";

const TABS = [
  { id: "overview", label: "Overview" },
  { id: "profile", label: "Profile" },
  { id: "listings", label: "My Listings" },
  { id: "selling", label: "Selling" },
  { id: "buying", label: "Buying" },
  { id: "fp-sales", label: "Fixed Price Sales" },
  { id: "fp-purchases", label: "Fixed Price Purchases" },
  { id: "watchlist", label: "Watchlist" },
  { id: "inbox", label: "Inbox" },
  { id: "ratings", label: "Ratings" },
] as const;
type TabId = (typeof TABS)[number]["id"];

export function MyAccountPage() {
  const [active, setActive] = useState<TabId>("overview");

  function renderTab() {
    switch (active) {
      case "overview":
        return <OverviewTab />;
      case "profile":
        return <ProfileTab />;
      case "listings":
        return <MyListingsTab />;
      case "selling":
        return <SellingTab />;
      case "buying":
        return <BuyingTab />;
      case "fp-sales":
        return <FixedPriceSalesTab />;
      case "fp-purchases":
        return <FixedPricePurchasesTab />;
      case "watchlist":
        return <WatchlistTab />;
      case "inbox":
        return <InboxPage />;
      case "ratings":
        return <RatingsTab />;
    }
  }

  return (
    <div className="max-w-6xl mx-auto">
      <h1 className="text-2xl font-semibold text-gray-900 mb-6">My Account</h1>
      {/* Tab bar */}
      <div className="flex gap-1 overflow-x-auto border-b mb-6 pb-0">
        {TABS.map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActive(tab.id)}
            className={cn(
              "px-4 py-2 text-sm font-medium whitespace-nowrap border-b-2 transition-colors",
              active === tab.id
                ? "border-orange-500 text-orange-600"
                : "border-transparent text-gray-500 hover:text-gray-800",
            )}
          >
            {tab.label}
          </button>
        ))}
      </div>
      {/* Tab content */}
      <div>{renderTab()}</div>
    </div>
  );
}

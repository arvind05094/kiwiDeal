import {
  Cpu,
  Shirt,
  Car,
  Sofa,
  Home,
  Dumbbell,
  BookOpen,
  Puzzle,
  Music,
  Archive,
  HelpCircle,
  Gauge,
} from "lucide-react";
import { CATEGORIES } from "@/shared/types/common";

const categoryIcons: Record<string, React.ReactNode> = {
  Electronics: <Cpu className="h-6 w-6" />,
  Clothing: <Shirt className="h-6 w-6" />,
  Motors: <Gauge className="h-6 w-6" />,
  Cars: <Car className="h-6 w-6" />,
  Furniture: <Sofa className="h-6 w-6" />,
  "Home and Garden": <Home className="h-6 w-6" />,
  Sports: <Dumbbell className="h-6 w-6" />,
  Books: <BookOpen className="h-6 w-6" />,
  Toys: <Puzzle className="h-6 w-6" />,
  Music: <Music className="h-6 w-6" />,
  Collectibles: <Archive className="h-6 w-6" />,
  Other: <HelpCircle className="h-6 w-6" />,
};

interface CategoryTilesProps {
  onSelect: (category: string) => void;
  selected?: string;
}

export function CategoryTiles({ onSelect, selected }: CategoryTilesProps) {
  return (
    <div className="grid grid-cols-3 gap-3 sm:grid-cols-4 md:grid-cols-6">
      {CATEGORIES.map((category) => (
        <button
          key={category}
          onClick={() => onSelect(category)}
          className={`flex flex-col items-center gap-2 rounded-lg border bg-white p-4 text-center transition-all hover:border-orange-400 hover:shadow-sm active:scale-95 ${
            selected === category
              ? "border-orange-500 text-orange-600"
              : "border-gray-200 text-gray-600"
          }`}
        >
          {categoryIcons[category]}
          <span className="text-xs font-medium leading-tight">{category}</span>
        </button>
      ))}
    </div>
  );
}

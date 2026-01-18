import { formatVND } from "@/utils/format";
import { Card } from "./ui/card";

interface FilterSidebarProps {
  priceRange: [number, number];
  onPriceChange: (range: [number, number]) => void;
  selectedType: string;
  onTypeChange: (type: string) => void;
  sortBy: string;
  onSortChange: (sort: string) => void;
}

// Giả lập danh sách kiểu phòng
const ROOM_TYPES = [
  { value: "all", label: "Tất cả hạng phòng" },
  { value: "standard", label: "Phòng Tiêu Chuẩn" },
  { value: "deluxe", label: "Phòng Deluxe Cao Cấp" },
  { value: "suite", label: "Phòng Suite Hạng Sang" },
  { value: "family", label: "Phòng Suite Gia Đình" },
];

// Giả lập các tùy chọn sắp xếp
const SORT_OPTIONS = [
  { value: "featured", label: "Nổi bật" },
  { value: "price-low", label: "Giá: Thấp đến cao" },
  { value: "price-high", label: "Giá: Cao đến thấp" },
  { value: "rating", label: "Đánh giá: Cao đến thấp" },
];

const FilterSidebar = ({
  priceRange,
  onPriceChange,
  selectedType,
  onTypeChange,
  sortBy,
  onSortChange,
}: FilterSidebarProps) => {
  return (
    <div className="space-y-6 sticky top-6">
      {/* Sort */}
      <Card className="border-border bg-card p-4 space-y-3">
        <h3 className="flex items-center gap-3 cursor-pointer hover:bg-secondary p-2 rounded transition-colors">
          Sắp xếp theo
        </h3>

        <div className="space-y-2">
          {SORT_OPTIONS.map((option) => (
            <label
              key={option.value}
              className="flex items-center gap-3 cursor-pointer hover:bg-secondary p-2 rounded transition-colors"
            >
              <input
                type="radio"
                name="sort"
                value={option.value}
                checked={sortBy === option.value}
                onChange={() => onSortChange(option.value)}
                className="h-4 w-4 rounded border-input text-accent focus:ring-2 focus:ring-ring"
              />
              <span className="text-sm text-foreground">{option.label}</span>
            </label>
          ))}
        </div>
      </Card>

      {/* Price Range */}
      <Card className="border-border bg-card p-4 space-y-4">
        <h3 className="text-sm font-medium text-foreground">Mức giá</h3>
        <div className="space-y-3">
          <div>
            <label className="text-xs text-muted-foreground">
              Từ: {formatVND(priceRange[0])}
            </label>
            <input
              type="range"
              min="0"
              max="20000000"
              step={500000}
              value={priceRange[0]}
              onChange={(e) =>
                onPriceChange([
                  Math.min(Number.parseInt(e.target.value), priceRange[1]),
                  priceRange[1],
                ])
              }
              className="w-full h-2 bg-secondary rounded-lg appearance-none cursor-pointer accent-accent"
            />
          </div>
          <div>
            <label className="text-xs text-muted-foreground">
              Đến: {formatVND(priceRange[1])}
            </label>
            <input
              type="range"
              min="0"
              max="20000000"
              step={500000}
              value={priceRange[1]}
              onChange={(e) =>
                onPriceChange([
                  priceRange[0],
                  Math.max(Number.parseInt(e.target.value), priceRange[0]),
                ])
              }
              className="w-full h-2 bg-secondary rounded-lg appearance-none cursor-pointer accent-accent"
            />
          </div>
        </div>
      </Card>

      {/* Room Type */}
      <Card className="border-border bg-card p-4 space-y-3">
        <h3 className="text-sm font-medium text-foreground">Kiểu phòng</h3>
        <div className="space-y-2">
          {ROOM_TYPES.map((type) => (
            <label
              key={type.value}
              className="flex items-center gap-3 cursor-pointer hover:bg-secondary p-2 rounded transition-colors"
            >
              <input
                type="radio"
                name="type"
                value={type.value}
                checked={selectedType === type.value}
                onChange={() => onTypeChange(type.value)}
                className="h-4 w-4 rounded border-input text-accent focus:ring-2 focus:ring-ring"
              />
              <span className="text-sm text-foreground">{type.label}</span>
            </label>
          ))}
        </div>
      </Card>
    </div>
  );
};

export default FilterSidebar;

# Seatify Typography System

## Font Family
The primary typeface for Seatify is **Inter** (falling back to *San Francisco* on Apple devices). This sans-serif font ensures excellent readability at small sizes while delivering a clean, modern, and tech-forward aesthetic.

- **Primary Font Stack:** `Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif`

## Font Weights
To keep the UI clean and prevent visual clutter, we restrict usage to three specific weights:
- **Regular (400):** Default body text, descriptions, and subtitles.
- **Medium (500):** Buttons, form input labels, card subheadings, and navigation links.
- **Semi-Bold (600):** Main headings (H1-H3), key highlights, and critical data points (e.g., total prices in the checkout cart).

## Typography Scale
Our typography scale aligns with a standard 8px grid system (Tailwind CSS defaults).

| Level       | Size (px/rem)  | Weight    | Line Height | Usage Context |
| :---        | :---           | :---      | :---        | :---          |
| **Display** | 48px / 3rem    | Semi-Bold | 1.2         | Landing page Hero text |
| **H1** | 36px / 2.25rem | Semi-Bold | 1.2         | Main page titles (e.g., Event Name) |
| **H2** | 24px / 1.5rem  | Semi-Bold | 1.3         | Section titles (e.g., Dashboard cards) |
| **H3** | 20px / 1.25rem | Semi-Bold | 1.4         | Modal titles |
| **Body Lg** | 18px / 1.125rem| Regular   | 1.5         | Landing page sub-headlines, intro texts |
| **Body** | 16px / 1rem    | Regular   | 1.5         | Standard paragraphs, descriptions |
| **UI Text** | 14px / 0.875rem| Medium    | 1.2         | Buttons, Navigation menus, Form labels |
| **Small** | 12px / 0.75rem | Regular   | 1.2         | Help text, captions, date stamps |

## Styling Rules
- **Color Contrast:** Headings always use the darkest shades (`color-dark-950` or `color-dark-900`). Body text uses slightly lighter shades (`color-dark-700` or `color-dark-600`) to establish visual hierarchy and reduce eye strain.
- **Alignment:** Default to **left-aligned** text for all standard UI components (forms, tables, lists). Center alignment should be used sparingly, reserved primarily for specific Landing Page sections or "Empty State" illustrations in the dashboard.

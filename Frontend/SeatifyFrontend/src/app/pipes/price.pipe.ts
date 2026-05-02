import { Pipe, PipeTransform } from '@angular/core';

/**
 * Formats a price with space as thousands separator.
 * Usage: {{ price | price }} → "12 000"
 *        {{ price | price:'HUF' }} → "12 000 HUF"
 */
@Pipe({
  name: 'price',
  standalone: false
})
export class PricePipe implements PipeTransform {
  transform(value: number | null | undefined, currency?: string): string {
    if (value == null) return '';

    // Format with space as thousands separator
    const formatted = Math.round(value)
      .toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, '\u00A0'); // non-breaking space

    return currency ? `${formatted} ${currency}` : formatted;
  }
}

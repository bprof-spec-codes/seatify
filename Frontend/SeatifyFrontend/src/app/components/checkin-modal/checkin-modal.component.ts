import { Component, OnDestroy, OnInit } from '@angular/core';
import { Html5QrcodeScanner } from 'html5-qrcode';
import { CheckInResult, CheckInService, TicketStatus } from '../../services/checkin.service';

@Component({
  selector: 'app-checkin-modal',
  templateUrl: './checkin-modal.component.html',
  styleUrls: ['./checkin-modal.component.sass'],
  standalone: false
})
export class CheckinModalComponent implements OnInit, OnDestroy {
  public isVisible = false;
  public manualTicketCode = '';
  public validationResult: CheckInResult | null = null;
  public processing = false;
  public errorMessage: string | null = null;
  
  private html5QrcodeScanner!: Html5QrcodeScanner;
  public TicketStatus = TicketStatus;
  public inputMode: 'camera' | 'manual' = 'camera';

  constructor(private checkInService: CheckInService) {}

  ngOnInit(): void {}

  ngOnDestroy(): void {
    if (this.html5QrcodeScanner) {
      try {
        this.html5QrcodeScanner.clear();
      } catch (e) {
        console.error(e);
      }
    }
  }

  public openModal(): void {
    this.isVisible = true;
    this.validationResult = null;
    this.errorMessage = null;
    this.manualTicketCode = '';
    this.inputMode = 'camera';
    this.initScanner();
  }

  public closeModal(): void {
    this.isVisible = false;
    this.stopScanner();
  }

  public toggleMode(): void {
    if (this.inputMode === 'camera') {
      this.inputMode = 'manual';
      this.stopScanner();
    } else {
      this.inputMode = 'camera';
      this.initScanner();
    }
  }

  private initScanner(): void {
    setTimeout(() => {
      this.html5QrcodeScanner = new Html5QrcodeScanner(
        "qr-reader",
        { fps: 10, qrbox: { width: 250, height: 250 } },
        /* verbose= */ false
      );
      this.html5QrcodeScanner.render((decodedText: string) => this.onScanSuccess(decodedText), () => {});
    }, 100);
  }

  private stopScanner(): void {
    if (this.html5QrcodeScanner) {
      try {
        this.html5QrcodeScanner.clear();
      } catch (e) {}
    }
  }

  private onScanSuccess(decodedText: string) {
    this.stopScanner();
    this.validateTicket(decodedText);
  }

  public validateManualCode(): void {
    if (this.manualTicketCode.trim().length > 0) {
      this.validateTicket(this.manualTicketCode.trim());
    }
  }

  private validateTicket(payload: string): void {
    this.processing = true;
    this.checkInService.validateTicket(payload).subscribe({
      next: (res) => {
        this.validationResult = res;
        this.processing = false;
        this.errorMessage = null;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = "Validation failed. Please check the code or your internet connection.";
        this.processing = false;
      }
    });
  }

  public confirmCheckIn(): void {
    if (this.validationResult && this.validationResult.status === TicketStatus.Valid) {
      this.processing = true;
      this.checkInService.confirmCheckIn(this.validationResult.ticketId).subscribe({
        next: (res) => {
          this.validationResult = res;
          this.processing = false;
        },
        error: (err) => {
          console.error(err);
          this.processing = false;
        }
      });
    }
  }

  public resetScanner(): void {
    this.validationResult = null;
    this.errorMessage = null;
    if (this.inputMode === 'camera') {
      this.initScanner();
    }
  }
}

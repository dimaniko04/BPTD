import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NgForm } from '@angular/forms';
import { FundraiserService } from 'src/app/services/fundraiser.service';
import { PaymentDto } from 'src/app/models/Fundraiser/PaymentDto';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';

@Component({
  selector: 'app-donation-modal',
  templateUrl: './donation-modal.component.html',
  styleUrls: ['./donation-modal.component.css']
})
export class DonationModalComponent {
  @Output() donationCompleted = new EventEmitter<number>();
  paymentModel: PaymentDto = {
    description: '',
    amount: '',
    card: '',
    cardExpiryDate: '',
    cardCvv: ''
  };
  isProcessing = false;

  constructor(
    private fundraiserService: FundraiserService,
    public dialogRef: MatDialogRef<DonationModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { fundraiserId: string; fundraiserTitle: string }
  ) {}

  donate(form: NgForm) {
    if (form.valid) {
      this.isProcessing = true;
      this.fundraiserService.donate(this.data.fundraiserId, this.paymentModel).subscribe({
        next: (updatedFundraiser : FundraiserDto) => {
          this.isProcessing = false;
          this.donationCompleted.emit(updatedFundraiser.amountRaised);
          this.closeModal();
        },
        error: (err) => {
          console.error('Donation failed:', err);
          this.isProcessing = false;
        }
      });
    }
  }

  closeModal() {
    this.dialogRef.close();
  }
}

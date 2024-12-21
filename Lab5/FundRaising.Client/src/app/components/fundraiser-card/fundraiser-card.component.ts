import { Component, Input } from '@angular/core';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';
import { DonationModalComponent } from '../donation-modal/donation-modal.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-fundraiser-card',
  templateUrl: './fundraiser-card.component.html',
  styleUrls: ['./fundraiser-card.component.css']
})
export class FundraiserCardComponent {
  @Input() fundraiser!: FundraiserDto;
  progressPercentage: number = 0;

  constructor(private matDialog: MatDialog) {}

  ngOnInit(): void {
    this.updateProgress();
  }

  updateProgress(): void {
    if (this.fundraiser.goal > 0) {
      this.progressPercentage = Math.min(
        (this.fundraiser.amountRaised / this.fundraiser.goal) * 100,
        100
      );
    } else {
      this.progressPercentage = 0;
    }
  }

  openDonationModal(): void {
    const dialogRef = this.matDialog.open(DonationModalComponent, {
      width: '500px',
      data: { fundraiserId: this.fundraiser.id, fundraiserTitle: this.fundraiser.title }
    });

    dialogRef.componentInstance.donationCompleted.subscribe((newAmountRaised: number) => {
      this.fundraiser.amountRaised = newAmountRaised;
      this.updateProgress();
    });
  }
}

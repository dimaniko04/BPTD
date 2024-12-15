import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Inject, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NgForm } from '@angular/forms';
import { FundraiserService } from 'src/app/services/fundraiser.service';
import { CreateFundraiserDto } from 'src/app/models/Fundraiser/CreateFundraiserDto';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';

@Component({
  selector: 'app-create-fundraiser-modal',
  templateUrl: './create-fundraiser-modal.component.html',
  styleUrls: ['./create-fundraiser-modal.component.css'],
})
export class CreateFundraiserModalComponent implements OnInit, AfterViewInit {
  fundraiserModel: CreateFundraiserDto = {
    title: '',
    description: '',
    goal: 0,
  };

  @ViewChild('createFundraiserForm') createForm: NgForm | undefined;
  @Output() fundraiserCreated = new EventEmitter<FundraiserDto>();

  constructor(
    private fundraiserService: FundraiserService,
    public dialogRef: MatDialogRef<CreateFundraiserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.cdr.detectChanges();
  }

  createFundraiser() {
    if (this.createForm?.valid) {
      this.fundraiserService.create(this.fundraiserModel).subscribe(
        (newFundraiser: FundraiserDto) => {
          this.fundraiserCreated.emit(newFundraiser);
          this.closeModal();
        },
        (error) => console.error('Error creating fundraiser:', error)
      );
    }
  }

  closeModal() {
    this.dialogRef.close();
  }
}

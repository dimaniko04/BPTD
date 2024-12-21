import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FundraiserService } from 'src/app/services/fundraiser.service';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';
import { Observable, BehaviorSubject, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { CreateFundraiserModalComponent } from '../create-fundraiser-modal/create-fundraiser-modal.component';

@Component({
  selector: 'app-fundraisers',
  templateUrl: './fundraisers.component.html',
  styleUrls: ['./fundraisers.component.css']
})
export class FundraisersComponent implements OnInit {
  dialogConfig = new MatDialogConfig();
  modalDialog: MatDialogRef<CreateFundraiserModalComponent, any> | undefined;

  private newFundraiserSubject = new BehaviorSubject<FundraiserDto | null>(null);

  fundraisers$?: Observable<FundraiserDto[]>;

  constructor(
    private fundraiserService: FundraiserService,
    public matDialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadFundraisers();
  }

  loadFundraisers(): void {
    this.fundraisers$ = combineLatest([
      this.fundraiserService.getAll(),
      this.newFundraiserSubject.asObservable()
    ]).pipe(
      map(([fundraisers, newFundraiser]) => {
        if (newFundraiser) {
          return [newFundraiser, ...fundraisers];
        }
        return fundraisers;
      })
    );
  }

  openCreateFundraiserForm(): void {
    this.dialogConfig.id = 'fundraisers-modal-component';
    this.dialogConfig = {
      width: '500px',
      panelClass: 'fundraisers-modal',
      data: {},
    };
  
    this.modalDialog = this.matDialog.open(CreateFundraiserModalComponent, this.dialogConfig);
  
    this.modalDialog.componentInstance.fundraiserCreated.subscribe((newFundraiser: FundraiserDto) => {
      this.newFundraiserSubject.next(newFundraiser);
    });
  }
  
}

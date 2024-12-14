import { Component, OnInit } from '@angular/core';
import { FundraiserService } from 'src/app/services/fundraiser.service';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-fundraisers',
  templateUrl: './fundraisers.component.html',
  styleUrls: ['./fundraisers.component.css']
})
export class FundraisersComponent implements OnInit {
  fundraisers$: Observable<FundraiserDto[]> | undefined;

  constructor(private fundraiserService: FundraiserService) {}

  ngOnInit(): void {
    this.loadFundraisers();
  }

  loadFundraisers(): void {
    this.fundraisers$ = this.fundraiserService.getAll();
  }
}
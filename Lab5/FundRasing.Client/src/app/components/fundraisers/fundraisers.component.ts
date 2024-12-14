import { Component, OnInit } from '@angular/core';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';
import { FundraiserService } from 'src/app/services/fundraiser.service';

@Component({
  selector: 'app-fundraisers',
  templateUrl: './fundraisers.component.html',
  styleUrls: ['./fundraisers.component.css']
})
export class FundraisersComponent implements OnInit {
  fundraisers: FundraiserDto[] = [];

  constructor(private fundraiserService: FundraiserService) {}

  ngOnInit(): void {
    this.loadFundraisers();
  }

  loadFundraisers(): void {
    this.fundraiserService.getAll().subscribe(data => {
      this.fundraisers = data;
    });
  }
}

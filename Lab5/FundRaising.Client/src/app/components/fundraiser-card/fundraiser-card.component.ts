import { Component, Input } from '@angular/core';
import { FundraiserDto } from 'src/app/models/Fundraiser/FundraiserDto';

@Component({
  selector: 'app-fundraiser-card',
  templateUrl: './fundraiser-card.component.html',
  styleUrls: ['./fundraiser-card.component.css']
})
export class FundraiserCardComponent {
  @Input() fundraiser!: FundraiserDto;
}

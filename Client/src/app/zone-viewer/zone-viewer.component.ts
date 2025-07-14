import { Component } from '@angular/core';
import { ZoneServiceService } from '../Service/zone-service.service';

@Component({
  selector: 'app-zone-viewer',
  templateUrl: './zone-viewer.component.html',
  styleUrls: ['./zone-viewer.component.css']
})
export class ZoneViewerComponent {
  
  imagePath: string | null = null;
  activeZone: string = '';
  /**
   *
   */
  constructor(public zoneService: ZoneServiceService) {
    
    
  }

  loadZone(zoneName: string)
  {
    this.activeZone = zoneName;
    this.imagePath = '/assets/Zone-Images/'+zoneName+'.png';
  }

}

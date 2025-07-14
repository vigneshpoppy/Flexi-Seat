import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ZoneViewerComponent } from './zone-viewer.component';

describe('ZoneViewerComponent', () => {
  let component: ZoneViewerComponent;
  let fixture: ComponentFixture<ZoneViewerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ZoneViewerComponent]
    });
    fixture = TestBed.createComponent(ZoneViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

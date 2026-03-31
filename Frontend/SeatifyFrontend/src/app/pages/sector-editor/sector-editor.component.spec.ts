import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SectorEditorComponent } from './sector-editor.component';

describe('SectorEditorComponent', () => {
  let component: SectorEditorComponent;
  let fixture: ComponentFixture<SectorEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SectorEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

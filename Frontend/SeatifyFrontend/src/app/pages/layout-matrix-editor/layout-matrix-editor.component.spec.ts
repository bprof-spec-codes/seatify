import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutMatrixEditorComponent } from './layout-matrix-editor.component';

describe('LayoutMatrixEditorComponent', () => {
  let component: LayoutMatrixEditorComponent;
  let fixture: ComponentFixture<LayoutMatrixEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LayoutMatrixEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LayoutMatrixEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

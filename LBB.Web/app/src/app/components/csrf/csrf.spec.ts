import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Csrf } from './csrf';

describe('Csrf', () => {
  let component: Csrf;
  let fixture: ComponentFixture<Csrf>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Csrf]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Csrf);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

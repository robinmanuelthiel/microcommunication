import { TestBed } from '@angular/core/testing';

import { RandomService } from './random.service';

describe('RandomService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RandomService = TestBed.get(RandomService);
    expect(service).toBeTruthy();
  });
});

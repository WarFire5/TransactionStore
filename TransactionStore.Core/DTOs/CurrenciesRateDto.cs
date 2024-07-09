﻿using TransactionStore.Core.Enums;

namespace TransactionStore.Core.DTOs;

public class CurrenciesRateDto
{
    public Guid Id { get; set; }
    public Currency Currency { get; set; }
    public decimal Rate { get; set; }
}
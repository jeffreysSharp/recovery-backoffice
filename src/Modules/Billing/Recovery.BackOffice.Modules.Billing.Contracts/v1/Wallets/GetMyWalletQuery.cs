using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Billing.Contracts.v1.Wallets;

public sealed record GetMyWalletQuery : IQuery<WalletDto>;

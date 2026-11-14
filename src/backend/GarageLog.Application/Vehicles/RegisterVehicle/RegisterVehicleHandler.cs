using GarageLog.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GarageLog.Application.Vehicles.RegisterVehicle;

public class RegisterVehicleHandler : IRequestHandler<RegisterVehicleCommand, RegisterVehicleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    public Task<RegisterVehicleResponse> Handle(RegisterVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}


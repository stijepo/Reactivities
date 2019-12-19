using System.Net;
using Domain;
using MediatR;
using Persistence;
using System.Threading.Tasks;
using System.Threading;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Application.Errors;

namespace Application.User
{
    public class Login
    {
        public class Query : IRequest<AppUser> 
        { 
            public string Email;
            public string Password;
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Query, AppUser>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            {
                _signInManager = signInManager;
                _userManager = userManager;
            }
        
            public async Task<AppUser> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    throw new RestException(HttpStatusCode.Unauthorized);

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.Succeeded)
                {
                    //TODO generate token
                    return user;
                }    

                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
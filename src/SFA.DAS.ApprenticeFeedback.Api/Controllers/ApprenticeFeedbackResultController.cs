﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsForAcademicYear;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary;
using SFA.DAS.ApprenticeFeedback.Domain.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ApprenticeFeedbackResultController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ApprenticeFeedbackResultController> _logger;

        public ApprenticeFeedbackResultController(IMediator mediator, ILogger<ApprenticeFeedbackResultController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateApprenticeFeedbackCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving apprentice feedback to database.");
                return BadRequest();
            }
        }

        [HttpGet("{ukprn}")]
        public async Task<IActionResult> GetApprenticeFeedbackDetails(long ukprn)
        {
            try
            {
                var result = await _mediator.Send(new GetApprenticeFeedbackDetailsQuery { Ukprn = ukprn });
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to retrieve apprentice feedback results for Ukprn: {ukprn}");
                return BadRequest();
            }
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetApprenticeFeedbackRatingSummary([RegularExpression("^(|All|AY\\d{4})$", ErrorMessage = "Time period should be empty, 'All', or in the format 'AYdddd'")] string timePeriod)
        {
            if (string.IsNullOrWhiteSpace(timePeriod))
            {
                timePeriod = ReviewDataPeriod.AggregatedData; ;
            }
            try
            {
                var result = await _mediator.Send(new GetApprenticeFeedbackRatingSummaryQuery() { TimePeriod = timePeriod });

                return Ok(result.RatingSummaries);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to retrieve apprentice feedback rating summaries");
                return BadRequest();
            }
        }

        [HttpGet("{ukprn}/annual")]
        public async Task<IActionResult> GetApprenticeFeedbackSummaryAnnual(long ukprn)
        {
            try
            {
                var result = await _mediator.Send(new GetApprenticeFeedbackDetailsAnnualQuery { Ukprn = ukprn });
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to retrieve annual apprentice feedback results for Ukprn: {ukprn}");
                return BadRequest();
            }
        }

        [HttpGet("{ukprn}/annual/{year}")]
        public async Task<IActionResult> GetApprenticeFeedbackSummaryAcademicYear(long ukprn, [RegularExpression(@"^AY\d{4}$", ErrorMessage = "Academic year should be in the format 'AYdddd'")] string year)
        {
            try
            {
                var result = await _mediator.Send(new GetApprenticeFeedbackDetailsForAcademicYearQuery { Ukprn = ukprn, AcademicYear = year });
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to retrieve annual apprentice feedback results for Ukprn: {ukprn} for year {year}");
                return BadRequest();
            }
        }
    }
}

/*
 <div class="text-center mb-3">
    <label class="form-check-label me-4">
        <input class="form-check-input task-filter" type="radio" name="taskFilter" value="Lead"> Lead
    </label>
    <label class="form-check-label">
        <input class="form-check-input task-filter" type="radio" name="taskFilter" value="Contact"> Contact
    </label>
</div>

<div id="taskFilteredResults" class="mt-3">
    <p class="text-muted">Please select a filter to view records.</p>
</div>
*/
/*
 public class TaskDto
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string WhoId { get; set; }
}
*/
/*
 <div class="text-center mb-3">
    <label class="form-check-label me-4">
        <input class="form-check-input task-filter" type="radio" name="taskFilter" value="Lead"> Lead
    </label>
    <label class="form-check-label">
        <input class="form-check-input task-filter" type="radio" name="taskFilter" value="Contact"> Contact
    </label>
</div>

<div id="taskFilteredResults" class="mt-3">
    <p class="text-muted">Please select a filter to view records.</p>
</div>
*/
/*
 public class DataController : Controller
{
    public IActionResult GetTaskFilterOptionsPartial()
    {
        return PartialView("_TaskFilterOptionsPartial");
    }
}
*/
/*
 case 'Task':
    url = '/Data/GetTaskFilterOptionsPartial'; // this will load radio buttons
    break;
*/
/*
 $(document).on('change', '.task-filter', function () {
    const selected = $(this).val();
    const $resultsDiv = $('#taskFilteredResults');

    $resultsDiv.html("<p class='text-muted'>Loading " + selected + " records with open tasks...</p>");

    let url = '';
    if (selected === 'Lead') {
        url = '/InProgress/GetLeadInProgress';
    } else if (selected === 'Contact') {
        url = '/InProgress/GetContactInProgress';
    }

    if (url) {
        $resultsDiv.load(url);
    }
});
*/
/*
 @model List<LeadOpenActivityDto>

<h5 class="text-primary mb-3">Leads with Open Tasks</h5>
<table class="table table-bordered">
    <thead>
        <tr><th>Name</th><th>Email</th><th>Open Tasks</th></tr>
    </thead>
    <tbody>
    @foreach (var lead in Model)
    {
        <tr>
            <td>@lead.FirstName @lead.LastName</td>
            <td>@lead.Email</td>
            <td>@lead.Tasks?.Count()</td>
        </tr>
    }
    </tbody>
</table>
*/
/*
 @model List<ContactInProgressDto>

<h5 class="text-success mb-3">Contacts with Open Tasks</h5>
<table class="table table-bordered">
    <thead>
        <tr><th>Name</th><th>Email</th><th>Open Tasks</th></tr>
    </thead>
    <tbody>
    @foreach (var contact in Model)
    {
        <tr>
            <td>@contact.FirstName @contact.LastName</td>
            <td>@contact.Email</td>
            <td>@contact.Tasks?.Count()</td>
        </tr>
    }
    </tbody>
</table>
*/
/*
 public async Task<IActionResult> GetLeadInProgress()
{
    var leads = await _inProgressService.GetLeadInProgressAsync();
    return PartialView("_LeadInProgressPartial", leads);
}

public async Task<IActionResult> GetContactInProgress()
{
    var contacts = await _inProgressService.GetContactInProgressAsync();
    return PartialView("_ContactInProgressPartial", contacts);
}
*/
/*
 case 'Task':
    url = '/Data/GetTaskTablePartial';
    $('#kendoSelectionContent').html(`
        <div class="text-center mb-3">
            <label class="form-check-label me-4">
                <input class="form-check-input task-filter" type="radio" name="taskFilter" value="Lead"> Lead
            </label>
            <label class="form-check-label">
                <input class="form-check-input task-filter" type="radio" name="taskFilter" value="Contact"> Contact
            </label>
        </div>
        <div id="taskFilteredResults" class="mt-3">
            <p class="text-muted">Please select a filter to view records.</p>
        </div>
    `);

    // Optionally bind click events if required for filter interaction
    $(document).on('change', 'input[name="taskFilter"]', function () {
        const selectedType = $(this).val();
        // Call your backend or update UI accordingly
        $('#taskFilteredResults').html(`<p class="text-muted">Loading ${selectedType} Tasks...</p>`);

        // Load partial view based on selection
        $.get(`/InProgress/Get${selectedType}InProgressPartial`, function (data) {
            $('#taskFilteredResults').html(data);
        });
    });
    return; // skip .load(url) as it's not needed for "Task"
*/
/*
 <script>
    $(document).ready(function () {
        $("#tabstrip").kendoTabStrip({
            animation: {
                open: { effects: "fadeIn" }
            }
        });
    });
</script>
*/
/*
 <style>
    .modal-content {
        border-radius: 0 !important;
    }

    .k-tabstrip {
        margin-top: -1rem;
    }

    .form-check-label {
        margin-left: 5px;
    }
</style>
*/
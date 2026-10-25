# Claims workflow - what I need

*From Priya Raman, Head of Claims, Harbourline Insurance*

I run a team of twelve handlers across four regions. Today a claim is just a record with a status dropdown. Anyone can set it to anything, and last quarter our auditors told me that is not good enough. I need the system to follow the way we actually handle a claim. I am not a technical person, so I will tell you how it works on the ground and you tell me what is sensible.

## How a claim should move

Every claim arrives as **New**. A handler then **triages** it: checks the policy was in force on the date of loss, sets the estimate and takes ownership. After that it is **assessed** (desk assessment, or an engineer or loss adjuster for the big ones). Then somebody **approves or declines** it. An approved claim is **settled**, and once the final payment has gone out it is **closed**.

Triage should happen quickly, ideally the same day the claim comes in. Claims that sit around too long at any stage need to be flagged to me. I do not want to find out from a complaint.

## Who is allowed to approve what

Approval depends on the handler's grade. Our grades and limits today are:

- Claims Assistant: up to 2,000
- Claims Handler: up to 10,000
- Senior Handler: up to 50,000
- Technical Lead: up to 250,000
- Anything above that comes to me personally.

If a claim is above your limit it goes up a grade. A handler must not be able to approve something above their limit, however they try to do it.

**The person who assessed a claim must never be the person who approves it.** No exceptions. The auditors love this one and I do not want to explain a breach.

In practice, South West and Wales only have two handlers, a Claims Assistant and one Claims Handler. When one of them is off the other has to be able to approve what they assessed themselves, up to their own limit, otherwise nothing moves. Just let them override it in that case.

## Settling, closing and reopening

A claim cannot be marked **Settled** unless there is a settlement amount on it. Declined and **Closed** are final. Once a claim is closed it is closed, and nobody can change it, not even me.

We do get new evidence, though: a second repair estimate, a third party coming back late, a recovery that falls through. So a settled or closed claim can be **reopened**, but only with a reason written down. We reopen quite a lot, and I want to see how often and why.

When a claim is settled, the system should pay the policyholder straight away by bank transfer to their account and text them to say the money is on its way. That saves my team a lot of chasing.

When we take the excess over the phone, the handler writes the card details on a sticky note, which is a nightmare. Let the handler put the full card number and the security code on the claim itself, so the next handler does not have to ring the customer again.

## What I want to see on screen

1. A **handler queue**: my team's claims, the oldest and most urgent first, so a handler knows what to pick up next. For any claim, I want one place to ask "what can happen to this claim now, and what is the next step?".
2. A **limits report** showing where approvals went above a handler's authority, so I can show the auditors.

## Other things on my mind

- The loss ratio report for the board does not feel right to me. Raj in finance quoted a different number for the same period. Please look at it while you are in there (ticket HLI-87).
- Reopened claims should keep their original claim number and their full history.
- The monthly bordereau from our delegated partners is imported by hand-uploading a CSV. Please do not break that.

Keep it simple. I would rather have the rules right than lots of screens.

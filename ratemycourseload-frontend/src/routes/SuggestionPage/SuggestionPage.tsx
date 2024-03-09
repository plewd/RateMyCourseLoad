import { Course } from '@types'
import {
    CircularProgress,
    Grid,
    Modal,
    ModalClose,
    ModalDialog,
    Typography,
} from '@mui/joy'
import React, {useEffect} from 'react'
import {API_ENDPOINT} from "../../App.tsx";

interface Props {
    coursesToRate: Course[]
}

const loadingMessages = [
    '🎓 Crunching numbers and brewing the perfect course load...',
    '📚 Sorting through the library of knowledge...',
    '🔬 Mixing academic potions for your perfect semester...',
    '🧠 Feeding the hamsters that power our course load calculator...',
    '📖 Turning pages and scanning chapters for your ideal courses...',
    '🕹️ Playing Tetris with your semester schedule...',
    '🚀 Launching your academic journey to the stars...',
    '🌐 Navigating the academic web to find your perfect fit...',
    '🏋️‍♂️ Pumping intellectual iron to shape your course load...',
    "🎲 Rolling the dice on your academic future... (Just kidding, we're professionals!)",
    '🔮 Gazing into the crystal ball of academia...',
    '🎈 Inflating your potential with the perfect course load...',
    '🍀 Searching for four-leaf clovers in the academic field...',
    '🎨 Painting a masterpiece with your course selections...',
    '🕵️ On a secret mission to find the best courses for you...',
    '🗺️ Charting your course through the sea of knowledge...',
    '🍳 Cooking up a feast of knowledge for your semester...',
    '🔎 Magnifying your academic potential...',
    '🦉 Summoning the wisdom of the academic owls...',
    '🏄‍♀️ Surfing the wave of knowledge to find your perfect courses...',
]

// Returns a number between 0..(max-1) (not inclusive of max)
const getRandomInt = (max: number) => Math.floor(Math.random() * max)

const getRandomLoadingMessage = () =>
    loadingMessages[getRandomInt(loadingMessages.length)]

const convertCourseListToStringList = (courses : Course[]) => (courses.map(course => course.Name));

interface CourseRatingResponse {
    "credit hours": string;
    "workload": string;
    "balance": string;
}

export default function SuggestionPage({ coursesToRate }: Props) {
    const [open, setOpen] = React.useState<boolean>(true)
    const [isLoading, setIsLoading] = React.useState<boolean>(true)
    const [response, setResponse] = React.useState<CourseRatingResponse | null>(null);

    useEffect(() => {
        const fetchRating = async () => {
            try {
                const courseStrings = convertCourseListToStringList(coursesToRate);
                const rawResponse = await fetch(`${API_ENDPOINT}/course/rate`, {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ courses: courseStrings })
                });
                const content = await rawResponse.json();
                console.log(content)
                setResponse(content);
                setIsLoading(false);
            } catch (error) {
                console.error('Error fetching course rating:', error);
                setIsLoading(false);
            }
        };

        fetchRating();
    }, [coursesToRate]);

    return (
        <Modal open={open} onClose={() => setOpen(false)}>
            <ModalDialog color="neutral" variant="soft">
                <ModalClose />
                <Grid
                    container
                    direction="row"
                    justifyContent="center"
                    alignItems="center"
                >
                    {isLoading ? (
                        <>
                            <CircularProgress
                                style={{
                                    marginLeft: '20vw',
                                    marginRight: '20vw',
                                    marginTop: '20vh',
                                    marginBottom: '20vh',
                                }}
                            />
                            <Typography>{getRandomLoadingMessage()}</Typography>
                        </>
                    ) : (
                        <Typography>
                            <strong>Credit Hours:</strong> {response?.["credit hours"]}<br/>
                            <strong>Workload:</strong> {response?.["workload"]}<br/>
                            <strong>Balance:</strong> {response?.["balance"]}
                        </Typography>
                    )}
                </Grid>
            </ModalDialog>
        </Modal>
    )
}
